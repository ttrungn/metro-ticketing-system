﻿using BuildingBlocks.Domain.Common;

namespace OrderService.Application.Carts.DTOs;

public class TicketReadModel : BaseReadModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int ExpirationInDay { get; set; }
    
    public decimal Price { get; set; }
    
    public int TicketType  { get; set; }

    public int ActiveInDay { get; set; }
}
