using NotificationService.Application.Common.Models;
using NotificationService.Application.Mails.Queries.SendCreateOrder;

namespace NotificationService.Application.Common.Interfaces;

public interface IOrderEmailBuilder
{
    Task<string> GenerateOrderTemplate(string email,
        List<OrderDetailRequestDto> orderDetails,
        CancellationToken cancellationToken = default);
}
