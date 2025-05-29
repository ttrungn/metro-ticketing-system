using YarpApiGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiGatewayServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("/api/sample/specification.json", "SampleService");
    c.SwaggerEndpoint("/api/auth/specification.json", "AuthService");
});

app.UseRouting();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

app.Run();