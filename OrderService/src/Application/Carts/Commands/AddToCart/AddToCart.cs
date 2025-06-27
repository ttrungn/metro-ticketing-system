using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Commands.AddToCart;

public record AddToCartCommand : IRequest<ServiceResponse<List<string>>>
{
    public string TicketId { get; init; } = null!;
    public int Quantity { get; init; }
    public string EntryStationId { get; init; } = null!;
    public string DestinationStationId { get; init; } = null!;
    public string RouteId { get; init; } = null!;
    
}
public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty().WithMessage("Hãy chọn vé.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");
        RuleFor(x => x.EntryStationId).NotEmpty().WithMessage("Hãy chọn ga đi.");
        RuleFor(x => x.DestinationStationId).NotEmpty().WithMessage("Hãy chọn ga đến.");
        RuleFor(x => x.RouteId).NotEmpty().WithMessage("Hãy chọn tuyến đường.");
    }
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, ServiceResponse<List<string>>>
{
    private readonly ILogger<AddToCartCommandHandler> _logger;
    private readonly ICartService _cartService;
    private readonly IUser _user;

    public AddToCartCommandHandler(ICartService cartService, ILogger<AddToCartCommandHandler> logger, IUser user)
    {
        _cartService = cartService;
        _logger = logger;
        _user = user;
    }

    public async Task<ServiceResponse<List<string>>> Handle(AddToCartCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _cartService.CreateAsync(request, _user.Id!, cancellationToken);

        if (customer == null || !customer.Any())
        {
            _logger.LogError("Failed to add item to cart for user {UserId}", _user.Id);
            return new ServiceResponse<List<string>> { Succeeded = false, Message = "Failed to add item to cart." };
        }

        _logger.LogInformation("Item added to cart for user {UserId}", _user.Id);
        return new ServiceResponse<List<string>>
        {
            Succeeded = true, Data = customer, Message = "Item added to cart successfully."
        };
    }
}
