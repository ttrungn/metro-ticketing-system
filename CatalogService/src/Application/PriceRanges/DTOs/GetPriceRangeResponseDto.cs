namespace CatalogService.Application.PriceRanges.DTOs;

public class GetPriceRangeResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; } = 8;
    public IEnumerable<PriceRangeDto> PriceRanges { get; set; } = new List<PriceRangeDto>();
}
