using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Infrastructure.Services;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HttpClientService> _logger;

    public HttpClientService(IHttpClientFactory httpClientFactory, ILogger<HttpClientService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<T> SendGet<T>(string baseUrl, string endpoint, CancellationToken cancellationToken = default)
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/{endpoint}");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", authorizationHeader);

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
