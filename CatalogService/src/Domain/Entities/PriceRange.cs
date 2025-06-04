using BuildingBlocks.Domain.Common;

namespace CatalogService.Domain.Entities;

public class PriceRange : BaseAuditableEntity<Guid>
{
    public int FromKm{ get; set; }
    public int ToKm{ get; set; }
    public decimal Price{ get; set; }   
}
