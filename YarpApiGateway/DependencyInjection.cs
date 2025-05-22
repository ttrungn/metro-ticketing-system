using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;
using YarpApiGateway.Exceptions;

namespace YarpApiGateway
{
    public static class DependencyInjection
    {
        public static void AddApiGatewayServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddExceptionHandler<CustomExceptionHandler>();
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            
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

            var globalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = configuration.GetValue<int>("RateLimiter:PermitLimit"),
                    Window = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimiter:WindowM")),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = configuration.GetValue<int>("RateLimiter:QueueLimit")
                });
            });

            services.AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"))
                .AddTransforms(transforms =>
                {
                    transforms.AddRequestTransform(async ctx =>
                    {
                        var h = ctx.ProxyRequest.Headers;
                        if (h.Authorization != null)
                        {
                            var result = await ctx.HttpContext
                                        .AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

                            if (!result.Succeeded)
                            {
                                throw new UnauthorizedAccessException("Invalid token");
                            }

                            var user = result.Principal!;
                            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
                            var email = user.FindFirstValue(ClaimTypes.Email);
                            var name = user.FindFirstValue(JwtRegisteredClaimNames.Name);
                            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

                            h.Add("X-User-Id", id ?? "");
                            h.Add("X-User-Email", email ?? "");
                            h.Add("X-User-Name", name ?? "");
                            h.Add("X-User-Roles", string.Join(",", roles));
                        }
                        await ValueTask.CompletedTask;
                    });
                });

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = globalLimiter;
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
        }
    }
}