using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Commands.AddToCart;

public record AddToCartCommand : IRequest<ServiceResponse<Guid>>
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
        if(string.IsNullOrEmpty(_user.Id))
        {
            _logger.LogWarning("User is not authenticated.");
            return new ServiceResponse<Guid>
            {
                Succeeded = false,
                Message = "Bạn cần đăng nhập để thực hiện thao tác này.",
                Data = Guid.Empty
            };
        }
        
        var cartId = await _cartService.CreateAsync(request, _user.Id!, cancellationToken);
        if (cartId == Guid.Parse("3631e38b-60dd-4d1a-af7f-a26f21c2ef82"))
        {
            _logger.LogError("Failed to add item to cart for user {UserId}", _user.Id);
            return new ServiceResponse<Guid>
            {
                Succeeded = true,
                Message = "Bạn phải đăng ký tài khoản student để thực hiện thao tác này.",
                Data = cartId
            };    
        }
        
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
