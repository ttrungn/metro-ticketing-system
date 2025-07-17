using OrderService.Application.Orders.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Common.Interfaces.Services;

public interface IOrderService
{
    Task<IEnumerable<TicketDto>> GetUserTicketsAsync(
        string? userId,
        PurchaseTicketStatus status,
        CancellationToken cancellationToken = default);

    Task<(Guid, Guid)> UpdateTicketAsync(
        string? userId,
        Guid id,
        Guid ticketId,
        PurchaseTicketStatus fromStatus,
        PurchaseTicketStatus? toStatus,
        CancellationToken cancellationToken = default);
}
