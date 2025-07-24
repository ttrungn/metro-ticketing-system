namespace NotificationService.Application.Common.Interfaces.Services;

public interface IHttpClientService
{
    Task<T> SendGet<T>(string baseUrl, string endpoint, CancellationToken cancellationToken = default);
    Task<T> SendPost<T>(string baseUrl, string endpoint, object content, CancellationToken cancellationToken = default);
}
