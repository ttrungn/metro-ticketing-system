using UserService.Application;
using UserService.Infrastructure;
using UserService.Infrastructure.Data;
using UserService.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api/user";
    settings.DocumentPath = "/api/user/specification.json";
});

app.MapFallbackToFile("index.html");

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/api/user"));

app.MapEndpoints();

app.Run();

public partial class Program
{
}
