using System.Linq.Expressions;
using BuildingBlocks.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderService.Application.Carts.Commands.AddToCart;
using OrderService.Application.Carts.DTOs;
using OrderService.Application.Common.Interfaces.Repositories;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientService _httpClientService;
    private readonly IConfiguration _configuration;
    
    public CartService(IUnitOfWork unitOfWork, IHttpClientService httpClientService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _httpClientService = httpClientService;
        _configuration = configuration;
    }
    
    public async Task<List<string>> CreateAsync(AddToCartCommand command, string userId, CancellationToken cancellationToken = default)
    {
        var baseUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"], message: "User Service Client URL is not configured.");
        var endpoint = $"api/user/Customers/";
        var response = await _httpClientService.SendGet<ServiceResponse<CustomerResponseDto>>(
            baseUrl,
            endpoint,
            cancellationToken: cancellationToken);
        if(response.Data == null)
        {
            return new List<string> { "Không tìm thấy thông tin người dùng." };
        }
        var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
        // Check if the user already has a same ticket in the cart
        var filter = new List<Expression<Func<Cart, bool>>>
        {
            c => c.CustomerId == response.Data.CustomerId
                 && c.TicketId == command.TicketId
                 && c.EntryStationId == command.EntryStationId
                 && c.DestinationStationId == command.DestinationStationId
        };
        var matchedItems  = await cartRepo.
                                                    FindAsync(filter, cancellationToken : cancellationToken);
        var existingCartItem = matchedItems.FirstOrDefault();
        
        if (existingCartItem != null)
        {
            existingCartItem.Quantity += command.Quantity;
            await cartRepo.UpdateAsync(existingCartItem, cancellationToken);
        }
        else
        {
            
            var newCart = new Cart
            {
                TicketId = command.TicketId.ToString(),
                Quantity = command.Quantity,
                EntryStationId = command.EntryStationId,
                DestinationStationId = command.DestinationStationId,
                RouteId = command.RouteId
            };
            newCart.Id = Guid.NewGuid();
            newCart.CustomerId = response.Data.CustomerId;
            await cartRepo.AddAsync(newCart, cancellationToken);
        }
        await _unitOfWork.SaveChangesAsync();
        return  new List<string> { "Thêm vào giỏ hàng thành công." };
    }

    public async Task<IEnumerable<CartResponseDto>> GetCartsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var baseUrlCustomer = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"], message: "User Service Client URL is not configured.");
        var endpointCustomer = $"api/user/Customers/";
        var responseCustomer = await _httpClientService.SendGet<ServiceResponse<CustomerResponseDto>>(
            baseUrlCustomer,
            endpointCustomer,
            cancellationToken: cancellationToken);
        if (responseCustomer.Data == null)
        {
            return new List<CartResponseDto>();
        }
        var customerId = responseCustomer.Data?.CustomerId;
        
        var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
        var carts = await cartRepo.Query() 
            .Where(c => c.CustomerId == customerId)
            .ToListAsync(cancellationToken);  

        
        var routeInfoLst = new Dictionary<string, string>(); 
        var entryStationNames = new Dictionary<string, string>(); 
        var destinationStationNames = new Dictionary<string, string>(); 
        var baseUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:CatalogServiceClient"], message: "Catalog Service Client URL is not configured.");

        foreach (var cart in carts)
        {
            var routeResponse = await _httpClientService.SendGet<ServiceResponse<RouteResponseDto>>(
                baseUrl, 
                $"api/catalog/Routes/{Guid.Parse(cart.RouteId)}",
                cancellationToken: cancellationToken);
            if (routeResponse.Data != null)
            {
                routeInfoLst[cart.RouteId] = routeResponse.Data!.Name!;
            }
            else
            {
                routeInfoLst[cart.RouteId] = "Unknown Route";
            }
            
            // Lấy thông tin về Entry Station
            var entryStationResponse = await _httpClientService.SendGet<ServiceResponse<StationResponseDto>>(
                baseUrl,
                $"api/catalog/stations/{Guid.Parse(cart.EntryStationId)}",
                cancellationToken: cancellationToken);
            if (entryStationResponse.Data != null)
            {
                entryStationNames[cart.EntryStationId] = entryStationResponse.Data!.Name!;
            }
            else
            {
                entryStationNames[cart.EntryStationId] = "Unknown Entry Station";
            }
            
            // Lấy thông tin về Destination Station
            var destinationStationResponse = await _httpClientService.SendGet<ServiceResponse<StationResponseDto>>(
                baseUrl,
                $"api/catalog/stations/{Guid.Parse(cart.DestinationStationId)}",
                cancellationToken: cancellationToken);
             
            if (destinationStationResponse.Data != null)
            {
                destinationStationNames[cart.DestinationStationId] = destinationStationResponse.Data!.Name!;
            }
            else
            {
                destinationStationNames[cart.DestinationStationId] = "Unknown Destination Station";
            }
            
        }
        var cartDtos = carts.Select(cart => new CartResponseDto
        {
            Ticket = cart.TicketId,
            EntryStationId = entryStationNames.GetValueOrDefault(cart.EntryStationId, "Unknown Entry Station"),
            DestinationStationId = destinationStationNames.GetValueOrDefault(cart.DestinationStationId, "Unknown Destination Station"),
            Route = routeInfoLst.GetValueOrDefault(cart.RouteId, "Unknown Route"),
            Quantity = cart.Quantity
        }).ToList();
        return cartDtos;
    
    }
}
