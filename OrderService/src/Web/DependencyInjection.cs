using BuildingBlocks.Domain.Events.Cart;
using BuildingBlocks.Domain.Events.Orders;
using BuildingBlocks.Domain.Events.Sample;
using Confluent.Kafka;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.OptionModel;
using OrderService.Infrastructure.Data;
using OrderService.Web.Consumers;
using OrderService.Web.Consumers.Cart;
using OrderService.Web.Services;

namespace OrderService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MomoOptionModel>()
                .Bind(configuration.GetSection("Momo"))
              .ValidateDataAnnotations()
               .ValidateOnStart();
        ;

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IUser, CurrentUser>();

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
            configure.Title = "OrderService API";

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
                rider.AddProducer<AddToCartEvent>(
                    configuration["KafkaSettings:OrderServiceEvents:AddToCart:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });
                rider.AddProducer<UpdateCartEvent>(
                    configuration["KafkaSettings:OrderServiceEvents:UpdateCart:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });
                rider.AddProducer<DeleteCartEvent>(
                    configuration["KafkaSettings:OrderServiceEvents:DeleteCart:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });
                
                rider.AddProducer<CreateOrderEvent>(
                    configuration["KafkaSettings:OrderServiceEvents:OrderCreated:Name"],
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
                rider.AddConsumer<AddToCartConsumer>();
                rider.AddConsumer<UpdateCartConsumer>();
                rider.AddConsumer<DeleteCartConsumer>();
                
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
                    k.TopicEndpoint<AddToCartEvent>(
                        configuration["KafkaSettings:OrderServiceEvents:AddToCart:Name"],
                        configuration["KafkaSettings:OrderServiceEvents:AddToCart:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<AddToCartConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<UpdateCartEvent>(
                        configuration["KafkaSettings:OrderServiceEvents:UpdateCart:Name"],
                        configuration["KafkaSettings:OrderServiceEvents:UpdateCart:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<UpdateCartConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<DeleteCartEvent>(
                        configuration["KafkaSettings:OrderServiceEvents:DeleteCart:Name"],
                        configuration["KafkaSettings:OrderServiceEvents:DeleteCart:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<DeleteCartConsumer>(context);
                        }
                    );
                });
            });
        });

        return services;
    }
}
