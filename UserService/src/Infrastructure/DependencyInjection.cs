using System.Text;
using Azure.Storage.Blobs;
using JasperFx;
using Marten;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Data.Interceptors;
using UserService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Common.Interfaces.Services;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Services.Identity;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var writeDbConnectionString = configuration.GetConnectionString("UserServiceWriteDb");
        var readDbConnectionString = configuration.GetConnectionString("UserServiceReadDb");
        var azureBlobStorageConnectionString = configuration["Azure:BlobStorageSettings:ConnectionString"];
        Guard.Against.NullOrEmpty(writeDbConnectionString, message: "Connection string 'UserServiceWriteDb' not found.");
        Guard.Against.NullOrEmpty(readDbConnectionString, message: "Connection string 'UserServiceReadDb' not found.");
        Guard.Against.Null(azureBlobStorageConnectionString, message: "Azure Blob Storage connection string not found. Make sure you have configured the connection");
        
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(writeDbConnectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddScoped<ApplicationDbContextInitialiser>();
        
        services.AddMarten(options =>
        {
            options.DisableNpgsqlLogging = true;

            options.Connection(readDbConnectionString);

            options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

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

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddSingleton(new BlobServiceClient(azureBlobStorageConnectionString));
        services.AddScoped<IAzureBlobService, AzureBlobService>();
        
        services.AddSingleton(TimeProvider.System);
        
        return services;
    }
}
