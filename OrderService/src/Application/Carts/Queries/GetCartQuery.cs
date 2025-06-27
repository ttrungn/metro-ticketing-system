using BuildingBlocks.Response;
using OrderService.Application.Carts.DTOs;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Queries;

public record GetCartQuery : IRequest<ServiceResponse<IEnumerable<CartResponseDto>>>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, 
    ServiceResponse<IEnumerable<CartResponseDto>>>
{
    private readonly ICartService _cartService;
    private readonly IUser _user;

    public GetCartQueryHandler(ICartService cartService, IUser user)
    {
        _cartService = cartService;
        _user = user;
    }

    public async Task<ServiceResponse<IEnumerable<CartResponseDto>>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var response = await _cartService.GetCartsAsync(_user.Id!, cancellationToken);
        return new ServiceResponse<IEnumerable<CartResponseDto>>()
        {
            Succeeded = true,
            Message = "Lấy giỏ hàng thành công.",
            Data = response
        };
    }

}
