using System.Text;
using Azure.Storage.Blobs;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Repositories;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Data.Interceptors;
using CatalogService.Infrastructure.Repositories;
using CatalogService.Infrastructure.Services;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CatalogService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var writeDbConnectionString = configuration.GetConnectionString("CatalogServiceWriteDb");
        var readDbConnectionString = configuration.GetConnectionString("CatalogServiceReadDb");
        var azureBlobStorageConnectionString = configuration["Azure:BlobStorageSettings:ConnectionString"];
        Guard.Against.Null(writeDbConnectionString, message: "Connection string 'CatalogServiceWriteDb' not found. Make sure you have configured the connection");
        Guard.Against.Null(readDbConnectionString, message: "Connection string 'CatalogServiceReadDb' not found. Make sure you have configured the connection");
        Guard.Against.Null(azureBlobStorageConnectionString, message: "Azure Blob Storage connection string not found. Make sure you have configured the connection");

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

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var key = configuration["JwtSettings:SecretKey"];
                Guard.Against.NullOrEmpty(key, message: "Cannot find JwtSettings:SecretKey in appsettings.json");
                var encodedKey = Encoding.ASCII.GetBytes(key);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(encodedKey)
                };
            });

        services.AddAuthorization();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddScoped(typeof(IMassTransitService<>), typeof(MassTransitService<>));
        services.AddSingleton(new BlobServiceClient(azureBlobStorageConnectionString));
        services.AddScoped<IAzureBlobService, AzureBlobService>();
        services.AddScoped<IPriceRangeService, PriceRangeService>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
