using BuildingBlocks.Response;
using CatalogService.Application.Tickets.DTO;
using Microsoft.Extensions.Configuration;
using NotificationService.Application.Common.Interfaces.CatalogServiceClient;
using NotificationService.Application.Common.Interfaces.Services;

namespace NotificationService.Infrastructure.Services;

public class CatalogServiceClient : ICatalogServiceClient
{
    private readonly IHttpClientService _httpClientService;
    private readonly string _baseUrl;
    
    public CatalogServiceClient(IHttpClientService httpClientService, IConfiguration configuration)
    {
        _httpClientService = httpClientService;
        _baseUrl = Guard.Against.NullOrEmpty(configuration["ClientSettings:CatalogServiceClient"],
            message: "Catalog Service Client URL is not configured.");
    }
    
    public async Task<ServiceResponse<GetTicketsResponseDto>> GetTicketsAsync(int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _httpClientService.SendGet<ServiceResponse<GetTicketsResponseDto>>(
            _baseUrl,
            $"api/catalog/Tickets/filter?page={page}&pageSize={pageSize}&status=false",
            cancellationToken: cancellationToken);
    }
    
    public async Task<ServiceResponse<GetStationsResponseDto>> GetStationsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _httpClientService.SendGet<ServiceResponse<GetStationsResponseDto>>(
            _baseUrl,
            $"api/catalog/Stations?page={page}&pageSize={pageSize}&status=false",
            cancellationToken: cancellationToken);
    }
}
