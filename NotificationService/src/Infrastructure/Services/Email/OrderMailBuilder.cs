using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.CatalogServiceClient;
using NotificationService.Application.Common.Interfaces.Services;
using NotificationService.Application.Common.Models;
using NotificationService.Application.Mails.Queries.SendCreateOrder;

namespace NotificationService.Infrastructure.Services.Email;

public class OrderMailBuilder : IOrderEmailBuilder
{
    private readonly ICatalogServiceClient _catalogServiceClient;

    public OrderMailBuilder(ICatalogServiceClient catalogServiceClient)
    {
        _catalogServiceClient = catalogServiceClient;
    }

    public async Task<string> GenerateOrderTemplate(string email,
        List<OrderDetailRequestDto> orderDetails,
        CancellationToken cancellationToken = default)
    {
        var orderMailData = await BuildOrderMailDataAsync(email, orderDetails, cancellationToken);
        
        var html = await File.ReadAllTextAsync("Templates/OrderSucceeded.html", cancellationToken);
        html = html.Replace("{{Amount}}", orderMailData.Amount.ToString("N0"));
        html = html.Replace("{{CustomerEmail}}", orderMailData.Email);
        var detail = orderMailData.OrderDetails.FirstOrDefault();
        if (detail != null)
        {
            html = html.Replace("{{TicketName}}", detail.TicketName);
            html = html.Replace("{{EntryStationName}}", detail.EntryStationName);
            html = html.Replace("{{DestinationStationName}}", detail.DestinationStationName);
            html = html.Replace("{{Quantity}}", detail.Quantity.ToString());
            html = html.Replace("{{Subtotal}}", detail.Price.ToString("N0"));
        }
        
        return html;
    }
    
    private async Task<OrderMailData> BuildOrderMailDataAsync(
        string email,
        List<OrderDetailRequestDto> orderDetails,
        CancellationToken cancellationToken = default)
    {
        var getTicketsResponse = await _catalogServiceClient.GetTicketsAsync(0, int.MaxValue, cancellationToken);
        var getStationsResponse = await _catalogServiceClient.GetStationsAsync(0, int.MaxValue, cancellationToken);
        var tickets = getTicketsResponse.Data?.Tickets?.ToList() ?? [];
        var stations = getStationsResponse.Data?.Stations?.ToList() ?? [];
        
        var orderDetailMails = new List<OrderDetailMailData>();
        double totalAmount = 0;

        foreach (var item in orderDetails)
        {
            var ticket = tickets?.FirstOrDefault(t => t.Id == item.TicketId);
            if (ticket is null) continue;

            var entryStation = stations?.FirstOrDefault(s => s.Id == item.EntryStationId);
            if (entryStation is null) continue;

            var destinationStation = stations?.FirstOrDefault(s => s.Id == item.DestinationStationId);
            if (destinationStation is null) continue;

            var ticketName = ticket.Name ?? "Unknown Ticket";
            var quantity = item.Quantity;
            var price = item.Price;

            totalAmount += (double)(price * quantity);

            orderDetailMails.Add(new OrderDetailMailData
            {
                TicketName = ticketName,
                EntryStationName = entryStation.Name ?? "Unknown Station",
                DestinationStationName = destinationStation.Name ?? "Unknown Station",
                Price = price,
                Quantity = quantity
            });
        }


        return new OrderMailData
        {
            Email = email,
            Amount = totalAmount,
            OrderDetails = orderDetailMails
        };
    }
}

