﻿using BuildingBlocks.Domain.Common;

namespace OrderService.Domain.Entities;

public class Cart : BaseAuditableEntity<(Guid CustomerId, Guid TicketId)>
{
    public string CustomerId { get; set; } = null!;
    public string TicketId { get; set; } = null!;
    public int Quantity { get; set; }
    public string EntryStationId { get; set; } = null!;
    public string DestinationStationId { get; set; } = null!;
    public string RouteId { get; set; } = null!;
}
