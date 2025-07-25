using System.Text;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Common.Interfaces.Repositories;
using OrderService.Application.Common.Interfaces.Services;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Data.Interceptors;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var writeDbConnectionString = configuration.GetConnectionString("OrderServiceWriteDb");
        var readDbConnectionString = configuration.GetConnectionString("OrderServiceReadDb");
        Guard.Against.Null(writeDbConnectionString, message: "Connection string 'OrderServiceWriteDb' not found. Make sure you have configured the connection");
        Guard.Against.Null(readDbConnectionString, message: "Connection string 'OrderServiceReadDb' not found. Make sure you have configured the connection");


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
        services.AddScoped<IMomoService, MomoService>();
        services.AddScoped<HttpClient>();
        services.AddHttpClient();

        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IOrderService, Services.OrderService>();
        services.AddScoped(typeof(IMassTransitService<>), typeof(MassTransitService<>));

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
