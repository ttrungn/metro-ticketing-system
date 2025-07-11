﻿using BuildingBlocks.Domain.Events.Buses;
using BuildingBlocks.Domain.Events.Routes;
using BuildingBlocks.Domain.Events.Sample;
using BuildingBlocks.Domain.Events.Stations;
using BuildingBlocks.Domain.Events.Tickets;
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
using CatalogService.Web.Consumers.Buses;
using CatalogService.Web.Consumers.Routes;
using CatalogService.Web.Consumers.Stations;
using CatalogService.Web.Consumers.Tickets;
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
        services.AddScoped<ITicketService, TicketService>();

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

                rider.AddProducer<CreateRouteEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:CreateRoute:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<UpdateRouteEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:UpdateRoute:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<DeleteRouteEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:DeleteRoute:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<CreateStationEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:CreateStation:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<UpdateStationEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:UpdateStation:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<DeleteStationEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:DeleteStation:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<CreateBusEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:CreateBus:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<UpdateBusEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:UpdateBus:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<DeleteBusEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:DeleteBus:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<CreateTicketEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:CreateTicket:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<UpdateTicketEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:UpdateTicket:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<DeleteTicketEvent>(
                    configuration["KafkaSettings:CatalogServiceEvents:DeleteTicket:Name"],
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

                rider.AddConsumer<CreateRouteConsumer>();
                rider.AddConsumer<UpdateRouteConsumer>();
                rider.AddConsumer<DeleteRouteConsumer>();

                rider.AddConsumer<CreateStationConsumer>();
                rider.AddConsumer<UpdateStationConsumer>();
                rider.AddConsumer<DeleteStationConsumer>();

                rider.AddConsumer<CreateBusConsumer>();
                rider.AddConsumer<UpdateBusConsumer>();
                rider.AddConsumer<DeleteBusConsumer>();

                rider.AddConsumer<CreateTicketConsumer>();
                rider.AddConsumer<UpdateTicketConsumer>();
                rider.AddConsumer<DeleteTicketConsumer>();

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
                    );
                    k.TopicEndpoint<CreateRouteEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:CreateRoute:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:CreateRoute:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateRouteConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<UpdateRouteEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:UpdateRoute:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:UpdateRoute:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<UpdateRouteConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<DeleteRouteEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:DeleteRoute:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:DeleteRoute:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<DeleteRouteConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<CreateStationEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:CreateStation:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:CreateStation:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateStationConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<UpdateStationEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:UpdateStation:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:UpdateStation:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<UpdateStationConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<DeleteStationEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:DeleteStation:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:DeleteStation:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<DeleteStationConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<CreateBusEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:CreateBus:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:CreateBus:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateBusConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<UpdateBusEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:UpdateBus:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:UpdateBus:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<UpdateBusConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<DeleteBusEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:DeleteBus:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:DeleteBus:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<DeleteBusConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<CreateTicketEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:CreateTicket:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:CreateTicket:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateTicketConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<UpdateTicketEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:UpdateTicket:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:UpdateTicket:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<UpdateTicketConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<DeleteTicketEvent>(
                        configuration["KafkaSettings:CatalogServiceEvents:DeleteTicket:Name"],
                        configuration["KafkaSettings:CatalogServiceEvents:DeleteTicket:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<DeleteTicketConsumer>(context);
                        }
                    );
                });
            });
        });

        return services;
    }
}
