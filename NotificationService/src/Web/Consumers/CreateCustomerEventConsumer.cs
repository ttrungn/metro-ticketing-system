using BuildingBlocks.Domain.Events.Users;
using MassTransit;
using NotificationService.Application.Mails.Queries.SendWelcome;

namespace NotificationService.Web.Consumers;

public class CreateCustomerEventConsumer : IConsumer<CreateCustomerEvent>
{
    private readonly ILogger<CreateCustomerEventConsumer> _logger;
    private readonly ISender _sender;
    
    public CreateCustomerEventConsumer(ILogger<CreateCustomerEventConsumer> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }
    
    public async Task Consume(ConsumeContext<CreateCustomerEvent> context)
    {
        var query = new SendWelcomeQuery()
        {
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
            Email = context.Message.Email
        };
            
        await _sender.Send(query);
    }
}
