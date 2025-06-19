namespace UserService.Application.Common.Interfaces.Services;

public interface IHttpClientService
{
    Task<T> SendGet<T>(string endpoint, string baseUrl, CancellationToken cancellationToken = default);
}
