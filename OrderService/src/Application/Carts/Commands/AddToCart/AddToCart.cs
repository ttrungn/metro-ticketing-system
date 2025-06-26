using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Commands.AddToCart;

public record AddToCartCommand : IRequest<ServiceResponse<Guid>>
{
    public string TicketId { get; init; } = null!;
    public int Quantity { get; init; }
    public string? EntryStationId { get; init; }
    public string? DestinationStationId { get; init; }
    public string? RouteId { get; init; }
    
}
public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty().WithMessage("Hãy chọn vé.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");
    }
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, ServiceResponse<Guid>>
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

    public async Task<ServiceResponse<Guid>> Handle(AddToCartCommand request,
        CancellationToken cancellationToken)
    {
        var cartId = await _cartService.CreateAsync(request, _user.Id!, cancellationToken);

        if (cartId == Guid.Empty)
        {
            _logger.LogError("Failed to add item to cart for user {UserId}", _user.Id);
            return new ServiceResponse<Guid>
            {
                Succeeded = false,
                Message = "Thêm vé vào giỏ hàng thất bại.",
                Data = Guid.Empty
            };    
        }

        _logger.LogInformation("Item added to cart for user {UserId}", _user.Id);
        return new ServiceResponse<Guid>
        {
            Succeeded = true,
            Message = "Thêm vé vào giỏ hàng thành công.",
            Data = cartId
        };    
    }
}
