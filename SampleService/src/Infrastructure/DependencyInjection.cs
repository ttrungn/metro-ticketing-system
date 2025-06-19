using System.Text;
using JasperFx;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
