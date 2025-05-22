using BuildingBlocks.Domain.Events.Sample;
using MassTransit;

namespace SampleService.Web.Consumers;

public class SampleConsumer : IConsumer<SampleEvent>
{
    private readonly ILogger<SampleConsumer> _logger;
    public SampleConsumer(ILogger<SampleConsumer> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<SampleEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("SampleConsumer Message Received");
        return Task.CompletedTask;
    }
}
