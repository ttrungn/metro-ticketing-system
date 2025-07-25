using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.CatalogServiceClient;
using NotificationService.Application.Common.Interfaces.Services;
using NotificationService.Application.Common.Models;
using NotificationService.Application.Mails.Queries.SendCreateOrder;

namespace NotificationService.Infrastructure.Services.Email;

public class OrderMailBuilder : IOrderEmailBuilder
{
    private readonly ILogger<OrderMailBuilder> _logger;
    private readonly ICatalogServiceClient _catalogServiceClient;

    public OrderMailBuilder(ILogger<OrderMailBuilder> logger, ICatalogServiceClient catalogServiceClient)
    {
        _logger = logger;
        _catalogServiceClient = catalogServiceClient;
    }

    public async Task<string> GenerateOrderTemplate(string email,
        List<OrderDetailRequestDto> orderDetails,
        CancellationToken cancellationToken = default)
    {
        var orderMailData = await BuildOrderMailDataAsync(email, orderDetails, cancellationToken);
        
        _logger.LogInformation("GenerateOrderTemplate: OrderMailData: {@OrderMailData}", JsonConvert.SerializeObject(orderMailData, Formatting.Indented));
        var html = await File.ReadAllTextAsync("Templates/OrderSucceeded.html", cancellationToken);
        html = html.Replace("{{Amount}}", orderMailData.Amount.ToString("N0"));
        html = html.Replace("{{CustomerEmail}}", orderMailData.Email);

        var orderDetailsRows = new StringBuilder();
        foreach (var detail in orderMailData.OrderDetails)
        {
            orderDetailsRows.AppendLine($@"
            <tr>
                <td>{detail.TicketName}</td>
                <td>{detail.EntryStationName}</td>
                <td>{detail.DestinationStationName}</td>
                <td>{detail.Quantity}</td>
                <td>{detail.Price.ToString("N0")} VND</td>
            </tr>");
        }

        html = html.Replace("{{OrderDetailsRows}}", orderDetailsRows.ToString());
        
        return html;
    }
    
    private async Task<OrderMailData> BuildOrderMailDataAsync(
        string email,
        List<OrderDetailRequestDto> orderDetails,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("BuildOrderMailDataAsync: OrderDetails: {@OrderDetails}", orderDetails);
        var getTicketsResponse = await _catalogServiceClient.GetTicketsAsync(0, int.MaxValue, cancellationToken);
        _logger.LogInformation("BuildOrderMailDataAsync: GetTicketsResponse: {@GetTicketsResponse}", getTicketsResponse);
        var getStationsResponse = await _catalogServiceClient.GetStationsAsync(0, int.MaxValue, cancellationToken);
        _logger.LogInformation("BuildOrderMailDataAsync: GetStationsResponse: {@GetStationsResponse}", getStationsResponse);
        var tickets = getTicketsResponse.Data?.Tickets?.ToList() ?? [];
        _logger.LogInformation("BuildOrderMailDataAsync: Tickets: {@Tickets}", tickets);
        var stations = getStationsResponse.Data?.Stations?.ToList() ?? [];
        _logger.LogInformation("BuildOrderMailDataAsync: Stations: {@Stations}", stations);
        
        var orderDetailMails = new List<OrderDetailMailData>();
        decimal totalAmount = 0;

        foreach (var item in orderDetails)
        {
            var ticket = tickets?.FirstOrDefault(t => t.Id == item.TicketId);
            if (ticket is null) continue;

            var entryStation = stations?.FirstOrDefault(s => s.Id == item.EntryStationId);

            var destinationStation = stations?.FirstOrDefault(s => s.Id == item.DestinationStationId);

            var ticketName = ticket.Name!;
            var quantity = item.Quantity;
            var price = item.Price;

            totalAmount += price;

            orderDetailMails.Add(new OrderDetailMailData
            {
                TicketName = ticketName,
                EntryStationName = entryStation?.Name ?? "Bất cứ trạm nào",
                DestinationStationName = destinationStation?.Name ?? "Bất cứ trạm nào",
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

