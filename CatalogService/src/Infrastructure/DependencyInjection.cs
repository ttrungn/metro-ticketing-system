using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Data.Interceptors;
using CatalogService.Infrastructure.Services;

namespace CatalogService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var writeDbConnectionString = configuration.GetConnectionString("CatalogServiceWriteDb");
        var readDbConnectionString = configuration.GetConnectionString("CatalogServiceReadDb");
        Guard.Against.Null(writeDbConnectionString, message: "Connection string 'WriteDb' not found. Make sure you have configured the connection");
        Guard.Against.Null(readDbConnectionString, message: "Connection string 'ReadDb' not found. Make sure you have configured the connection");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(writeDbConnectionString);
        });

        services.AddMarten(options =>
            {
                options.DisableNpgsqlLogging = true;
            
            // Establish the connection string to your Marten database
            options.Connection(readDbConnectionString);

            // Specify that we want to use STJ as our serializer
            options.UseSystemTextJsonForSerialization();
        })
        .UseLightweightSessions();
        
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddScoped(typeof(IMassTransitService<>), typeof(MassTransitService<>));
        
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
