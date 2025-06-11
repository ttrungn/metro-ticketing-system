namespace CatalogService.Application.Buses.DTOs;

public class BusResponseDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public Guid StationId { get; set; }
    public string? DestinationName { get; set; }
}
