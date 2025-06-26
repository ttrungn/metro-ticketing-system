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
    
    public async Task<Guid> CreateAsync(AddToCartCommand command, string userId, CancellationToken cancellationToken = default)
    {
        var response = await GetCustomerResponse(userId, cancellationToken);
        if(response.Data == null)
        {
            return Guid.Empty;
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
        Guid cartId;
        if (existingCartItem != null)
        {
            existingCartItem.Quantity += command.Quantity;
            await cartRepo.UpdateAsync(existingCartItem, cancellationToken);
            cartId = existingCartItem.Id;
        }
        else
        {
            cartId = Guid.NewGuid();
            var newCart = new Cart
            {
                TicketId = command.TicketId.ToString(),
                Quantity = command.Quantity,
                EntryStationId = command.EntryStationId,
                DestinationStationId = command.DestinationStationId,
                RouteId = command.RouteId
            };
            newCart.Id = cartId;
            newCart.CustomerId = response.Data.CustomerId;
            await cartRepo.AddAsync(newCart, cancellationToken);
        }
        await _unitOfWork.SaveChangesAsync();
        return  cartId;
    }

    public async Task<IEnumerable<CartResponseDto>?> GetCartsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var responseCustomer = await GetCustomerResponse(userId, cancellationToken);
        if (responseCustomer.Data == null)
        {
            return null;
        }
        var customerId = responseCustomer.Data?.CustomerId;
        
        var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
        var carts = await cartRepo.Query() 
            .Where(c => c.CustomerId == customerId)
            .ToListAsync(cancellationToken);  

        
        var routeInfoLst = new Dictionary<string, string>(); 
        var entryStationNames = new Dictionary<string, string>(); 
        var destinationStationNames = new Dictionary<string, string>(); 
        
        await PopulateCartDetails(carts, routeInfoLst, entryStationNames, destinationStationNames, cancellationToken);
        
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


    #region Helper Methods
    private async Task PopulateCartDetails(IEnumerable<Cart> carts, Dictionary<string, string> routeInfoLst, 
        Dictionary<string, string> entryStationNames, Dictionary<string, string> destinationStationNames, 
        CancellationToken cancellationToken)
    {
        var baseUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:CatalogServiceClient"], message: "Catalog Service Client URL is not configured.");

        foreach (var cart in carts)
        {
            // Fetch route information
            var routeResponse = await GetRouteInfo(cart.RouteId, baseUrl, cancellationToken);
            routeInfoLst[cart.RouteId] = routeResponse ?? "Unknown Route";

            // Fetch entry station information
            var entryStationResponse = await GetStationInfo(cart.EntryStationId, baseUrl, cancellationToken);
            entryStationNames[cart.EntryStationId] = entryStationResponse ?? "Unknown Entry Station";

            // Fetch destination station information
            var destinationStationResponse = await GetStationInfo(cart.DestinationStationId, baseUrl, cancellationToken);
            destinationStationNames[cart.DestinationStationId] = destinationStationResponse ?? "Unknown Destination Station";
        }
    }
    private async Task<ServiceResponse<CustomerResponseDto>> GetCustomerResponse(string userId, CancellationToken cancellationToken)
    {
        var baseUrlCustomer = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"], message: "User Service Client URL is not configured.");
        var endpointCustomer = $"api/user/Customers/";
        return await _httpClientService.SendGet<ServiceResponse<CustomerResponseDto>>(
            baseUrlCustomer,
            endpointCustomer,
            cancellationToken: cancellationToken);
    }
    private async Task<string?> GetRouteInfo(string routeId, string baseUrl, CancellationToken cancellationToken)
    {
        var routeResponse = await _httpClientService.SendGet<ServiceResponse<RouteResponseDto>>(
            baseUrl,
            $"api/catalog/Routes/{Guid.Parse(routeId)}",
            cancellationToken: cancellationToken);

        return routeResponse?.Data?.Name;
    }

    private async Task<string?> GetStationInfo(string stationId, string baseUrl, CancellationToken cancellationToken)
    {
        var stationResponse = await _httpClientService.SendGet<ServiceResponse<StationResponseDto>>(
            baseUrl,
            $"api/catalog/stations/{Guid.Parse(stationId)}",
            cancellationToken: cancellationToken);

        return stationResponse?.Data?.Name;
    }
    

    #endregion
}
