using System.Reflection;
using SampleService.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using SampleService.Domain.Entities;

namespace SampleService.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
