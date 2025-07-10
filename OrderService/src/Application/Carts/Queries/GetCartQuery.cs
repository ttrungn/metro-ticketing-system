using BuildingBlocks.Response;
using OrderService.Application.Carts.DTOs;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Queries;

public record GetCartQuery : IRequest<ServiceResponse<IEnumerable<GetCartsResponseDto>>>;
public class GetCartQueryHandler : IRequestHandler<GetCartQuery, 
    ServiceResponse<IEnumerable<GetCartsResponseDto>>>
{
    private readonly ICartService _cartService;
    private readonly IUser _user;

    public GetCartQueryHandler(ICartService cartService, IUser user)
    {
        _cartService = cartService;
        _user = user;
    }

    public async Task<ServiceResponse<IEnumerable<GetCartsResponseDto>>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(_user.Id))
        {
            return new ServiceResponse<IEnumerable<GetCartsResponseDto>>()
            {
                Succeeded = false,
                Message = "Bạn cần đăng nhập để thực hiện thao tác này.",
                Data = null
            };
        }
        var response = await _cartService.GetCartsAsync(_user.Id!, cancellationToken);
        return new ServiceResponse<IEnumerable<GetCartsResponseDto>>()
        {
            Succeeded = true,
            Message = "Lấy giỏ hàng thành công.",
            Data = response
        };
    }
}
