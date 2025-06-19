namespace UserService.Application.Common.Interfaces.Services;

public interface IHttpClientService
{
    Task<T> SendRequest<T>(string endpoint, HttpMethod method, Object? body = null, CancellationToken cancellationToken = default);
}
