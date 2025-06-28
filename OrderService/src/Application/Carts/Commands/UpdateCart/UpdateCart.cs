
using BuildingBlocks.Response;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Commands.UpdateCart;
public record UpdateCartCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; init; }
    public int Quantity { get; init; }
}
public class UpdateCartCommandValidator : AbstractValidator<UpdateCartCommand>
{
    public UpdateCartCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID giỏ hàng!");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0!");
    }
}
public class UpdateCartCommandHandler : IRequestHandler<UpdateCartCommand, ServiceResponse<Guid>>
{
    private readonly ICartService _cartService;
    private readonly IUser _user;

    public UpdateCartCommandHandler(ICartService cartService, IUser user)
    {
        _cartService = cartService;
        _user = user;
    }

    public async Task<ServiceResponse<Guid>> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_user.Id))
        {
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Bạn cần đăng nhập để thực hiện thao tác này.",
                Data = Guid.Empty
            };
        }

        var response = await _cartService.UpdateCartAsync(request.Id, request.Quantity, _user.Id!, cancellationToken);

        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Cập nhật giỏ hàng thành công.",
            Data = response
        };
    }
}
