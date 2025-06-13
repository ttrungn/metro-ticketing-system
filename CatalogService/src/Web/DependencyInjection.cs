using BuildingBlocks.Domain.Events.Sample;
using Confluent.Kafka;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Common.Interfaces.Services;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Services;
using CatalogService.Web.Consumers;
using CatalogService.Web.Services;

namespace CatalogService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IUser, CurrentUser>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IStationRouteService, StationRouteService>();    
        services.AddScoped<IStationService, StationService>();
        services.AddScoped<IBusService, BusService>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "CatalogService API";

            // Add JWT
            configure.AddSecurity("JWT", Enumerable.Empty<string>(),
                new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });

        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddProducer<SampleEvent>(
                    configuration["KafkaSettings:SampleEvents:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddConsumer<SampleConsumer>();
                rider.UsingKafka((context, k) =>
                {
                    k.Host(configuration["KafkaSettings:Url"], h =>
                    {
                        if (
                            configuration["KafkaSettings:Username"] != null &&
                            configuration["KafkaSettings:Username"] != string.Empty &&
                            configuration["KafkaSettings:Password"] != null &&
                            configuration["KafkaSettings:Password"] != string.Empty
                        )
                        {

                            h.UseSasl(s =>
                            {
                                s.Mechanism = SaslMechanism.Plain;
                                s.SecurityProtocol = SecurityProtocol.SaslSsl;
                                s.Username = configuration["KafkaSettings:Username"];
                                s.Password = configuration["KafkaSettings:Password"];
                            });
                        }
                    });

                    k.TopicEndpoint<SampleEvent>(
                        configuration["KafkaSettings:SampleEvents:Name"],
                        configuration["KafkaSettings:SampleEvents:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<SampleConsumer>(context);
                        }
                    );;
                });
            });
        });

        return services;
    }
}
