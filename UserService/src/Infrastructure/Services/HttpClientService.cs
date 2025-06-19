using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Infrastructure.Services;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;
    private readonly ILogger<HttpClientService> _logger;
    private readonly IUser _user;

    public HttpClientService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<HttpClientService> logger, IUser user)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _user = user;
        _apiBaseUrl = configuration["ClientSettings:ApiBaseUrl"] ?? "http://localhost:8081";
    }

    public async Task<T> SendRequest<T>(string endpoint, HttpMethod method, Object? body = null, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(method, $"{_apiBaseUrl}/{endpoint}");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("X-User-Id", _user.Id);
        request.Headers.Add("X-User-Email", _user.Id);
        request.Headers.Add("X-User-Name", _user.Id);
        request.Headers.Add("X-User-Role", _user.Id);

        if (body != null && method != HttpMethod.Get && method != HttpMethod.Delete)
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );
        }

        var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Request to {Endpoint} failed with status code {StatusCode}", endpoint, response.StatusCode);
            return default!;
        }

        _logger.LogInformation("Request to {Endpoint} succeeded with status code {StatusCode}", endpoint, response.StatusCode);
        var responseData = await response.Content.ReadFromJsonAsync<T>(cancellationToken);

        return responseData!;
    }
}
