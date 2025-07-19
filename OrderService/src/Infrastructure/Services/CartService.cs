using System.Linq.Expressions;
using BuildingBlocks.Domain.Events.Cart;
using BuildingBlocks.Response;
using Marten;
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
    
    public async Task<CartCreatedResponse> CreateAsync(AddToCartCommand command, string userId, CancellationToken cancellationToken = default)
    {
        Guid cartId;
        var responseCustomer = await GetCustomerResponse(userId, cancellationToken);
  
        var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
        
        var responseTicket = await GetTicketInfo(command.TicketId, cancellationToken);
        // Check if account is student add the student ticket
        if (responseCustomer.Data!.IsStudent == false)
        {
            if (responseTicket!.TicketType == 3)
            {
                return new CartCreatedResponse() { IsStudent = false };
            }
        }
        // Check if the user already has a same ticket in the cart
        var filter = new List<Expression<Func<Cart, bool>>>
        {
            c => c.CustomerId == responseCustomer.Data.CustomerId.ToString()
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
            existingCartItem.AddDomainEvent(new AddToCartEvent()
            {
                Id = existingCartItem.Id,
                CustomerId = existingCartItem.CustomerId,
                TicketId = existingCartItem.TicketId,
                Quantity = existingCartItem.Quantity,
                EntryStationId = existingCartItem.EntryStationId,
                DestinationStationId = existingCartItem.DestinationStationId,
                RouteId = existingCartItem.RouteId
            });
            cartId = existingCartItem.Id;
        }
        else
        {
            cartId = Guid.NewGuid();
            var newCart = new Cart()
            {
                Id = cartId,
                TicketId = command.TicketId,
                Quantity = command.Quantity,
                EntryStationId = command.EntryStationId,
                DestinationStationId = command.DestinationStationId,
                RouteId = command.RouteId,
                CustomerId = responseCustomer.Data.CustomerId.ToString()
            };
            await cartRepo.AddAsync(newCart, cancellationToken);
            
            newCart.AddDomainEvent(new AddToCartEvent()
            {
                Id = newCart.Id,
                CustomerId = newCart.CustomerId,
                TicketId = newCart.TicketId,
                Quantity = newCart.Quantity,
                EntryStationId = newCart.EntryStationId,
                DestinationStationId = newCart.DestinationStationId,
                RouteId = newCart.RouteId
            });
        }
        await _unitOfWork.SaveChangesAsync();
        return  new CartCreatedResponse
        {
            Id = cartId,
            IsStudent = responseCustomer.Data.IsStudent
        };
    }
    
    public async Task<IEnumerable<GetCartsResponseDto>?> GetCartsAsync(string userId, CancellationToken cancellationToken = default)
    {

        var responseCustomer = await GetCustomerResponse(userId, cancellationToken);
        if (responseCustomer.Data == null)
        {
            return null;
        }
        var customerId = responseCustomer.Data?.CustomerId;
        var session = _unitOfWork.GetDocumentSession();
        var cartReadModels = await QueryableExtensions.ToListAsync(
            session.Query<CartReadModel>()
                .Where(c => c.CustomerId == customerId.ToString())
                .AsNoTracking(),
            cancellationToken);
        
        var routeInfoLst = new Dictionary<string, string>(); 
        var entryStationNames = new Dictionary<string, string>(); 
        var destinationStationNames = new Dictionary<string, string>(); 
        var ticketPrices = new Dictionary<string, decimal>();
        var ticketNames = new Dictionary<string, string>();
        
        await PopulateCartDetails(cartReadModels, routeInfoLst, entryStationNames, destinationStationNames, ticketNames, ticketPrices, cancellationToken);
        
        var cartDtos = cartReadModels.Select(cart => new GetCartsResponseDto
        {
            CartId = cart.Id.ToString(),
            TicketName = ticketNames.GetValueOrDefault(cart.TicketId!, "Unknown Ticket"),
            EntryStationName = entryStationNames.GetValueOrDefault(cart.EntryStationId!, ""),
            DestinationStationName = destinationStationNames.GetValueOrDefault(cart.DestinationStationId!, ""),
            Route = routeInfoLst.GetValueOrDefault(cart.RouteId!, ""),
            Quantity = cart.Quantity,
            Price = ticketPrices.GetValueOrDefault(cart.TicketId!, 0),
        }).ToList();
        return cartDtos;
    
    }

    public async Task<int> GetQuantitiesCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var responseCustomer = await GetCustomerResponse(userId, cancellationToken);
        var customerId = responseCustomer.Data?.CustomerId;
        
        var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
        var carts = await EntityFrameworkQueryableExtensions.ToListAsync(cartRepo.Query() 
                .Where(c => c.CustomerId == customerId.ToString()), cancellationToken);  

        return carts.Sum(c => c.Quantity);
    }

    public async Task<Guid> UpdateCartAsync(Guid id,int quantity, string userId, 
                                            CancellationToken cancellationToken = default)
    {
        var responseCustomer = await GetCustomerResponse(userId, cancellationToken);
        if (responseCustomer.Data == null)
        {
            return Guid.Empty;
        }
        
        var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
        var cart = await cartRepo.GetByIdAsync(id, cancellationToken);
        
        if (cart == null)
        {
            return Guid.Empty; 
        }

        cart.Quantity = quantity;
        cart.AddDomainEvent(new UpdateCartEvent()
        {
            Id = cart.Id,
            Quantity = cart.Quantity
        });
        await cartRepo.UpdateAsync(cart, cancellationToken);
        await cartRepo.SaveChangesAsync(cancellationToken);
        
        return cart.Id;
    }
    public async Task<Guid> DeleteCartAsync(Guid id, string userId, 
        CancellationToken cancellationToken = default)
    {
        var responseCustomer = await GetCustomerResponse(userId, cancellationToken);
        if (responseCustomer.Data == null)
        {
            return Guid.Empty;
        }
        
        var cartRepo = _unitOfWork.GetRepository<Cart, Guid>();
        var cart = await cartRepo.GetByIdAsync(id, cancellationToken);
        
        if (cart == null)
        {
            return Guid.Empty; 
        }
        cart.AddDomainEvent(new DeleteCartEvent()
        {
            Id = cart.Id
        });
        
        await cartRepo.RemoveOutAsync(cart, cancellationToken);
        await cartRepo.SaveChangesAsync(cancellationToken);
        return cart.Id;
    }
    #region Helper Methods
    private async Task<ServiceResponse<CustomerReadModel>> GetCustomerResponse(string userId, CancellationToken cancellationToken)
    {
        var baseUrlCustomer = Guard.Against.NullOrEmpty(_configuration["ClientSettings:UserServiceClient"], message: "User Service Client URL is not configured.");
        var endpointCustomer = $"api/user/Customers/profile";
        return await _httpClientService.SendGet<ServiceResponse<CustomerReadModel>>(
            baseUrlCustomer,
            endpointCustomer,
            cancellationToken: cancellationToken);
    }

    private async Task PopulateCartDetails(IEnumerable<CartReadModel> carts, Dictionary<string, string> routeInfoLst, 
        Dictionary<string, string> entryStationNames, Dictionary<string, string> destinationStationNames, 
        Dictionary<string, string> ticketNames, Dictionary<string, decimal> ticketPrices,
        CancellationToken cancellationToken)
    {
        var baseUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:CatalogServiceClient"], message: "Catalog Service Client URL is not configured.");

        foreach (var cart in carts)
        {
            // Fetch route information
            var routeResponse = await GetRouteInfo(cart.RouteId!, baseUrl, cancellationToken);
            routeInfoLst[cart.RouteId!] = routeResponse ?? "";

            // Fetch entry station information
            var entryStationResponse = await GetStationInfo(cart.EntryStationId!, baseUrl, cancellationToken);
            entryStationNames[cart.EntryStationId!] = entryStationResponse ?? "";

            // Fetch destination station information
            var destinationStationResponse = await GetStationInfo(cart.DestinationStationId!, baseUrl, cancellationToken);
            destinationStationNames[cart.DestinationStationId!] = destinationStationResponse ?? "";
            
            // Fetch destination ticket information
            if (routeResponse == null || entryStationResponse == null || destinationStationResponse == null)
            { 
                var ticketResponse = await GetTicketInfo(cart.TicketId!, cancellationToken);
                ticketNames[cart.TicketId!] = ticketResponse!.Name ?? "Unknown Ticket"; 
                ticketPrices[cart.TicketId!] = ticketResponse.Price;
            }
            else
            {
                var ticketResponseSingleUse =
                    await GetTicketSingleUse(new GetTicketInfoRequestDto
                        {
                            RouteId = Guid.Parse(cart.RouteId!),
                            EntryStationId = Guid.Parse(cart.EntryStationId!),
                            ExitStationId = Guid.Parse(cart.DestinationStationId!)
                        }, 
                        baseUrl, 
                        cancellationToken); 
                ticketNames[cart.TicketId!] = ticketResponseSingleUse!.Name ?? "Unknown Ticket"; 
                ticketPrices[cart.TicketId!] = ticketResponseSingleUse.Price;
                
            }
       
        }
    }
        private async Task<string?> GetRouteInfo(string routeId, string baseUrl, CancellationToken cancellationToken)
    {    
        if (string.IsNullOrEmpty(routeId) || !Guid.TryParse(routeId, out var routeGuid))
        {
            return null;
        }
        var routeResponse = await _httpClientService.SendGet<ServiceResponse<RouteResponseDto>>(
            baseUrl,
            $"api/catalog/Routes/{Guid.Parse(routeId)}",
            cancellationToken: cancellationToken);

        return routeResponse?.Data?.Name ?? "Route Not Found";
    }

    private async Task<string?> GetStationInfo(string stationId, string baseUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(stationId) || !Guid.TryParse(stationId, out var stationGuid))
        {
            return null;
        }
        var stationResponse = await _httpClientService.SendGet<ServiceResponse<StationReadModel>>(
            baseUrl,
            $"api/catalog/stations/{Guid.Parse(stationId)}",
            cancellationToken: cancellationToken);

        return stationResponse?.Data?.Name ?? "Station Not Found";
    }
    private async Task<TicketReadModel?> GetTicketSingleUse(GetTicketInfoRequestDto request, string baseUrl, CancellationToken cancellationToken)
    {
        var ticketResponse = await _httpClientService.SendPost<ServiceResponse<TicketReadModel>>(
            baseUrl,
            $"api/catalog/Tickets/single-use-ticket-info/",
            request,
            cancellationToken: cancellationToken);

        return ticketResponse?.Data;
    }
    
    private async Task<TicketReadModel?> GetTicketInfo(string ticketId, CancellationToken cancellationToken)
    {
        var baseUrl = Guard.Against.NullOrEmpty(_configuration["ClientSettings:CatalogServiceClient"], message: "Catalog Service Client URL is not configured.");
        var ticketResponse = await _httpClientService.SendGet<ServiceResponse<TicketReadModel>>(
            baseUrl,
            $"api/catalog/Tickets/{ticketId}",
            cancellationToken: cancellationToken);

        return ticketResponse?.Data;
    }

    #endregion
}
