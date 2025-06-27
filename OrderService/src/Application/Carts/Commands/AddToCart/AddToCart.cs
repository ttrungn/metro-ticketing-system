using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using OrderService.Application.Carts.DTOs;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Commands.AddToCart;

public record AddToCartCommand : IRequest<ServiceResponse<CartIdResponseWithStudentDto>>
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

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, ServiceResponse<CartIdResponseWithStudentDto>>
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
    
    public async Task<ServiceResponse<CartIdResponseWithStudentDto>> Handle(AddToCartCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_user.Id))
        {
            _logger.LogWarning("User is not authenticated.");
            return new ServiceResponse<CartIdResponseWithStudentDto>
            {
                Succeeded = false,
                Message = "Bạn cần đăng nhập để thực hiện thao tác này.",
                Data = null
            };
        }

        var cart = await _cartService.CreateAsync(request, _user.Id!, cancellationToken);

        if (cart == null)
        {
            _logger.LogError("Failed to add item to cart for user {UserId}", _user.Id);
            return new ServiceResponse<CartIdResponseWithStudentDto>
            {
                Succeeded = false,
                Message = "Thêm vé vào giỏ hàng thất bại.",
                Data = null
            };
        }

        _logger.LogInformation("Item added to cart for user {UserId}", _user.Id);
        return new ServiceResponse<CartIdResponseWithStudentDto>
        {
            Succeeded = true,
            Message = "Thêm vé vào giỏ hàng thành công.",
            Data = cart
        };
    }
}
