using OrderService.Application.Carts.Commands.AddToCart;
using OrderService.Application.Carts.DTOs;

namespace OrderService.Application.Common.Interfaces.Services;

public interface ICartService
{
    Task<List<string>> CreateAsync(AddToCartCommand command, string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CartResponseDto>> GetCartsAsync(string userId, CancellationToken cancellationToken = default);
}
