namespace UserService.Application.Common.Interfaces.Services;

public interface IHttpClientService
{
    Task<T> SendGet<T>(string baseUrl, string endpoint, CancellationToken cancellationToken = default);
}
