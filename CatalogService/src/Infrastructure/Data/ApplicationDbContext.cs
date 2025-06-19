using System.Reflection;
using CatalogService.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using CatalogService.Domain.Entities;

namespace CatalogService.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<StationRoute> StationRoutes => Set<StationRoute>();
    public DbSet<Bus> Buses => Set<Bus>();
    public DbSet<PriceRange> PriceRanges => Set<PriceRange>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
