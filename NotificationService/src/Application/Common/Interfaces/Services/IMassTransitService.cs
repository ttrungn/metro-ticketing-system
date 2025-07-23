namespace NotificationService.Application.Common.Interfaces.Services;

public interface IMassTransitService<in TMessage>
{
    Task Produce(TMessage message, CancellationToken cancellationToken = default);
}
