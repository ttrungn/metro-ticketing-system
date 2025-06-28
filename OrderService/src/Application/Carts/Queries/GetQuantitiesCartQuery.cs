using BuildingBlocks.Response;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.Carts.Queries;

public record GetQuantitiesCartQuery : IRequest<ServiceResponse<int>>;

public class GetQuantitiesCartQueryHandler : IRequestHandler<GetQuantitiesCartQuery, ServiceResponse<int>>
{
    private readonly ICartService _cartService;
    private readonly IUser _user;

    public GetQuantitiesCartQueryHandler(ICartService cartService, IUser user)
    {
        _cartService = cartService;
        _user = user;
    }

    public async Task<ServiceResponse<int>> Handle(GetQuantitiesCartQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_user.Id))
        {
            return new ServiceResponse<int>()
            {
                Succeeded = false,
                Message = "Bạn cần đăng nhập để thực hiện thao tác này.",
                Data = 0
            };
        }
        
        var response = await _cartService.GetQuantitiesCartAsync(_user.Id!, cancellationToken);
        
        return new ServiceResponse<int>()
        {
            Succeeded = true,
            Message = "Lấy số lượng sản phẩm trong giỏ hàng thành công.",
            Data = response
        };
    }
}
