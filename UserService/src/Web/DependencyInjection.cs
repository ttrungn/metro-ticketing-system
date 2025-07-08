using BuildingBlocks.Domain.Events.Feedbacks;
using BuildingBlocks.Domain.Events.FeedbackTypes;
using BuildingBlocks.Domain.Events.Users;
using Confluent.Kafka;
using MassTransit;
using UserService.Application.Common.Interfaces;
using UserService.Infrastructure.Data;
using UserService.Web.Services;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using UserService.Application.Common.Interfaces.Services;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Services.StudentRequests;
using UserService.Web.Consumers;
using UserService.Web.Consumers.Feedbacks;
using UserService.Web.Consumers.FeedbackTypes;
using UserService.Web.Consumers.StudentRequest;

namespace UserService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IUser, CurrentUser>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddTransient<IStudentRequestService, StudentRequestService>();
        services.AddHttpContextAccessor();

        services.AddHttpClient();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "UserService API";

            // Add JWT
            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
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
                rider.AddProducer<CreateCustomerEvent>(
                    configuration["KafkaSettings:UserServiceEvents:CreateCustomer:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<CreateStaffEvent>(
                    configuration["KafkaSettings:UserServiceEvents:CreateStaff:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<CreateFeedbackTypeEvent>(
                    configuration["KafkaSettings:UserServiceEvents:CreateFeedbackType:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<UpdateFeedbackTypeEvent>(
                    configuration["KafkaSettings:UserServiceEvents:UpdateFeedbackType:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<DeleteFeedbackTypeEvent>(
                    configuration["KafkaSettings:UserServiceEvents:DeleteFeedbackType:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });

                rider.AddProducer<CreateFeedbackEvent>(
                    configuration["KafkaSettings:UserServiceEvents:CreateFeedback:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });
                
                rider.AddProducer<CreateStudentRequestEvent>(
                    configuration["KafkaSettings:UserServiceEvents:CreateStudentRequest:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });
                
                rider.AddProducer<UpdateStudentRequestApproveEvent>(
                    configuration["KafkaSettings:UserServiceEvents:UpdateStudentRequestApprove:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });
                rider.AddProducer<UpdateStudentRequestDeclinedEvent>(
                    configuration["KafkaSettings:UserServiceEvents:UpdateStudentRequestDeclined:Name"],
                    (ctx, kc) =>
                    {
                        kc.MessageTimeout = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageTimeoutMs"));
                        kc.MessageSendMaxRetries =
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:MessageSendMaxRetries");
                        kc.RetryBackoff = TimeSpan.FromMilliseconds(
                            configuration.GetValue<int>("KafkaSettings:ProducerConfigs:RetryBackoffMs"));
                    });
                rider.AddConsumer<CreateCustomerConsumer>();
                rider.AddConsumer<CreateStaffConsumer>();

                rider.AddConsumer<CreateFeedbackTypeConsumer>();
                rider.AddConsumer<UpdateFeedbackTypeConsumer>();
                rider.AddConsumer<DeleteFeedbackTypeConsumer>();

                rider.AddConsumer<CreateFeedbackConsumer>();
                
                rider.AddConsumer<CreateStudentRequestConsumer>();
                rider.AddConsumer<UpdateStudentRequestApproveEventConsumer>();
                rider.AddConsumer<UpdateStudentRequestDeclinedEventConsumer>();
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

                    k.TopicEndpoint<CreateCustomerEvent>(
                        configuration["KafkaSettings:UserServiceEvents:CreateCustomer:Name"],
                        configuration["KafkaSettings:UserServiceEvents:CreateCustomer:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateCustomerConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<CreateStaffEvent>(
                        configuration["KafkaSettings:UserServiceEvents:CreateStaff:Name"],
                        configuration["KafkaSettings:UserServiceEvents:CreateStaff:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateStaffConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<CreateFeedbackTypeEvent>(
                        configuration["KafkaSettings:UserServiceEvents:CreateFeedbackType:Name"],
                        configuration["KafkaSettings:UserServiceEvents:CreateFeedbackType:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateFeedbackTypeConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<UpdateFeedbackTypeEvent>(
                        configuration["KafkaSettings:UserServiceEvents:UpdateFeedbackType:Name"],
                        configuration["KafkaSettings:UserServiceEvents:UpdateFeedbackType:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<UpdateFeedbackTypeConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<DeleteFeedbackTypeEvent>(
                        configuration["KafkaSettings:UserServiceEvents:DeleteFeedbackType:Name"],
                        configuration["KafkaSettings:UserServiceEvents:DeleteFeedbackType:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<DeleteFeedbackTypeConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<CreateFeedbackEvent>(
                        configuration["KafkaSettings:UserServiceEvents:CreateFeedback:Name"],
                        configuration["KafkaSettings:UserServiceEvents:CreateFeedback:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateFeedbackConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<CreateStudentRequestEvent>(
                        configuration["KafkaSettings:UserServiceEvents:CreateStudentRequest:Name"],
                        configuration["KafkaSettings:UserServiceEvents:CreateStudentRequest:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<CreateStudentRequestConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<UpdateStudentRequestApproveEvent>(
                        configuration["KafkaSettings:UserServiceEvents:UpdateStudentRequestApprove:Name"],
                        configuration["KafkaSettings:UserServiceEvents:UpdateStudentRequestApprove:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<UpdateStudentRequestApproveEventConsumer>(context);
                        }
                    );
                    k.TopicEndpoint<UpdateStudentRequestDeclinedEvent>(
                        configuration["KafkaSettings:UserServiceEvents:UpdateStudentRequestDeclined:Name"],
                        configuration["KafkaSettings:UserServiceEvents:UpdateStudentRequestDeclined:Group"],
                        e =>
                        {
                            e.ConfigureConsumer<UpdateStudentRequestDeclinedEventConsumer>(context);
                        }
                    );
                });
            });
        });

        return services;
    }
}
