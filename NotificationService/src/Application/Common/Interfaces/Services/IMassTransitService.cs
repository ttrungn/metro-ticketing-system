namespace NotificationService.Application.Common.Interfaces.Services;

public interface IMassTransitService<TMessage>
{
    Task Produce(TMessage message, CancellationToken cancellationToken = default);
}
