using YarpApiGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiGatewayServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/api/sample/specification.json", "SampleService");
    c.SwaggerEndpoint("/api/catalog/specification.json", "CatalogService");
    c.SwaggerEndpoint("/api/order/specification.json", "OrderService");
    c.SwaggerEndpoint("/api/notification/specification.json", "NotificationService");
    c.SwaggerEndpoint("/api/user/specification.json", "UserService");
});

app.UseRouting();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

app.Run();