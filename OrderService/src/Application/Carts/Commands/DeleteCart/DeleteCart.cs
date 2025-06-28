using BuildingBlocks.Response;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Commands.DeleteCart;

public record DeleteCartCommand(Guid Id) : IRequest<ServiceResponse<Guid>>;
public class DeleteCartCommandValidator : AbstractValidator<DeleteCartCommand>
{
    public DeleteCartCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Xin vui lòng nhập ID giỏ hàng!");
    }
}
public class DeleteCartCommandHandler : IRequestHandler<DeleteCartCommand, ServiceResponse<Guid>>
{
    private readonly ICartService _cartService;
    private readonly IUser _user;

    public DeleteCartCommandHandler(ICartService cartService, IUser user)
    {
        _cartService = cartService;
        _user = user;
    }

    public async Task<ServiceResponse<Guid>> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
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

        var response = await _cartService.DeleteCartAsync(request.Id, _user.Id!, cancellationToken);

        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Xoá giỏ hàng thành công.",
            Data = response
        };
    }
}
