using JasperFx;
using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleService.Application.Common.Interfaces;
using SampleService.Application.Common.Interfaces.Repositories;
using SampleService.Application.Common.Interfaces.Services;
using SampleService.Infrastructure.Data;
using SampleService.Infrastructure.Data.Interceptors;
using SampleService.Infrastructure.Repositories;
using SampleService.Infrastructure.Services;

namespace SampleService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var writeDbConnectionString = configuration.GetConnectionString("SampleServiceWriteDb");
        var readDbConnectionString = configuration.GetConnectionString("SampleServiceReadDb");
        Guard.Against.Null(writeDbConnectionString, message: "Connection string 'SampleServiceWriteDb' not found. Make sure you have configured the connection");
        Guard.Against.Null(readDbConnectionString, message: "Connection string 'SampleServiceReadDb' not found. Make sure you have configured the connection");

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

            options.Connection(readDbConnectionString);

            options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

            options.UseSystemTextJsonForSerialization();
        })
        .UseLightweightSessions();
        
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddScoped(typeof(IMassTransitService<>), typeof(MassTransitService<>));
        
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
