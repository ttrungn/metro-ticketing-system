namespace CatalogService.Application.PriceRanges.DTOs;

public class PriceRangeDto
{
    public Guid Id { get; set; }
    public int FromKm{ get; set; }
    public int ToKm{ get; set; }
    public decimal Price{ get; set; }  
    public bool DeleteFlag { get; set; }
}
