using BuildingBlocks.Domain.Events.Sample;
using Microsoft.Extensions.Logging;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Services;

namespace OrderService.Application.WeatherForecasts.EventHandlers;

public class SampleEventHandler : INotificationHandler<SampleEvent>
{
    private readonly ILogger<SampleEventHandler> _logger;
    private readonly IMassTransitService<SampleEvent> _massTransitService; 
    public SampleEventHandler(ILogger<SampleEventHandler> logger, IMassTransitService<SampleEvent> massTransitService)
    {
        _logger = logger;
        _massTransitService = massTransitService;
    }
    public async Task Handle(SampleEvent sampleEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending SampleEvent to Kafka");
        await _massTransitService.Produce(sampleEvent, cancellationToken);
        _logger.LogInformation("SampleEvent sent to Kafka");
    }
}
