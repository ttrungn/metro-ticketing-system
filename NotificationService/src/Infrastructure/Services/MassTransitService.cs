using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Exceptions;
using NotificationService.Application.Common.Interfaces.Services;

namespace NotificationService.Infrastructure.Services;

public class MassTransitService<TMessage> : IMassTransitService<TMessage> where TMessage : class
{
    private readonly ILogger<MassTransitService<TMessage>> _logger;
    private readonly ITopicProducer<TMessage> _producer;

    public MassTransitService(ILogger<MassTransitService<TMessage>> logger, ITopicProducer<TMessage> producer)
    {
        _logger = logger;
        _producer = producer;
    }

    public async Task Produce(TMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Trying to send {EventName} to Kafka topic", typeof(TMessage).Name);
            await _producer.Produce(message, cancellationToken);
            _logger.LogInformation("Successfully sent {EventName} to Kafka topic", typeof(TMessage).Name);
        }
        catch (ProduceException<byte[], byte[]> ex)
        {
            _logger.LogError("@{Exception}", ex);
            throw new ServiceUnavailableException(
                "Unable to deliver message to Kafka at this time, please try again later.");
        }
    }
}
