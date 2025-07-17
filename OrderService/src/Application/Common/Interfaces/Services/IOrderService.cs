using OrderService.Application.Orders.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Common.Interfaces.Services;

public interface IOrderService
{
    Task<IEnumerable<TicketDto>> GetUserTicketsAsync(string? userId, PurchaseTicketStatus status, CancellationToken cancellationToken = default);
}
