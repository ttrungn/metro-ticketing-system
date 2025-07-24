using BuildingBlocks.Domain.Events.Orders;
using BuildingBlocks.Domain.Utils;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.Services;
using NotificationService.Application.Common.Models;

namespace NotificationService.Application.Mails.Queries.SendCreateOrder;

public record SendCreateOrderQuery : IRequest<Unit>
{
    public string Email { get; init; } = null!;
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public List<OrderDetailRequestDto> OrderDetails { get; init; } = null!;
}

public class SendCreateOrderValidator : AbstractValidator<SendCreateOrderQuery>
{
    public SendCreateOrderValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Amount).NotEmpty();
        RuleFor(x => x.OrderDetails).NotEmpty();
    }
}

public class SendCreateOrderHandler : IRequestHandler<SendCreateOrderQuery, Unit>
{
    private readonly ILogger<SendCreateOrderHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly IUserEmailBuilder _userEmailBuilder;
    private readonly IOrderEmailBuilder _orderEmailBuilder;
    
    public SendCreateOrderHandler(ILogger<SendCreateOrderHandler> logger, IEmailService emailService, IUserEmailBuilder userEmailBuilder, IOrderEmailBuilder orderEmailBuilder)
    {
        _logger = logger;
        _emailService = emailService;
        _userEmailBuilder = userEmailBuilder;
        _orderEmailBuilder = orderEmailBuilder;
    }

    public async Task<Unit> Handle(SendCreateOrderQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending email to {Email} for order {OrderId}", request.Email, request.OrderId);
        await _emailService.SendMailAsync(new MailData()
        {
            EmailToName = request.Email,
            EmailToId = request.Email,
            EmailBody = await _orderEmailBuilder.GenerateOrderTemplate(request.Email, request.OrderDetails, cancellationToken),
            EmailSubject = "Đơn hàng của bạn đã được thanh toán thành công",
        });

        return Unit.Value;
    }
}
