using OrderService.Application.Carts.Commands.AddToCart;
using OrderService.Application.Carts.DTOs;

namespace OrderService.Application.Common.Interfaces.Services;

public interface ICartService
{
    Task<CartIdResponseWithStudentDto> CreateAsync(AddToCartCommand command, string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CartResponseDto>?> GetCartsAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> GetQuantitiesCartAsync(string userId, CancellationToken cancellationToken = default);

    Task<Guid> DeleteCartAsync(Guid id, string userId, CancellationToken cancellationToken = default);

    Task<Guid> UpdateCartAsync(Guid id, int quantity, string userId, CancellationToken cancellationToken = default);
}
