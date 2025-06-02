using BuildingBlocks.Domain.Common;

namespace NotificationService.Domain.Entities;

public class WeatherForecast : BaseAuditableEntity<Guid>
{
    public required DateTime Date { get; set; }
    public required int TemperatureC { get; set; }
    public required string Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
