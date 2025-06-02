using BuildingBlocks.Domain.Events.Sample;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SampleService.Application.Common.Interfaces;
using SampleService.Application.Common.Interfaces.Services;
using SampleService.Domain.Entities;

namespace SampleService.Infrastructure.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApplicationDbContext _context;
    private readonly IDocumentSession _documentSession;

    public WeatherForecastService(IHttpContextAccessor httpContextAccessor, IApplicationDbContext context, IDocumentSession documentSession)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _documentSession = documentSession;
    }

    public async Task<Guid> CreateAsync(DateTime date, int temperatureC, string summary, CancellationToken cancellationToken = default)
    {
        var entity = new WeatherForecast()
        {
            Date = date,
            TemperatureC = temperatureC,
            Summary = summary
        };
        entity.AddDomainEvent(new SampleEvent());

        _context.WeatherForecasts.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
    
    public async Task UpdateAsync(Guid id, DateTime date, int temperatureC, string summary, CancellationToken cancellationToken = default)
    {
        var entity = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.WeatherForecasts, w => w.Id == id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(WeatherForecast), id.ToString());

        entity.Date = date;
        entity.TemperatureC = temperatureC;
        entity.Summary = summary;
        entity.AddDomainEvent(new SampleEvent());
        
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.WeatherForecasts, w => w.Id == id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(WeatherForecast), id.ToString());

        entity.DeleteFlag = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<WeatherForecast>().ToListAsync(token: cancellationToken);
    }
}
