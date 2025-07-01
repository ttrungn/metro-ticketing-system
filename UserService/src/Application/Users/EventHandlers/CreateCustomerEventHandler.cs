using BuildingBlocks.Domain.Events.Users;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Users.EventHandlers;

public class CreateCustomerEventHandler : INotificationHandler<CreateCustomerEvent>
{
    private readonly ILogger<CreateCustomerEventHandler> _logger;
    private readonly IMassTransitService<CreateCustomerEvent> _massTransitService;
    
    public CreateCustomerEventHandler(ILogger<CreateCustomerEventHandler> logger, IMassTransitService<CreateCustomerEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }
    
    public async Task Handle(CreateCustomerEvent createCustomerEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing CreateCustomerEvent to the message bus for customer with Id: {CustomerId}", createCustomerEvent.CustomerId);
        await _massTransitService.Produce(createCustomerEvent, cancellationToken);
    }
}
