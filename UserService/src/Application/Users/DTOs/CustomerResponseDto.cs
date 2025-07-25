﻿using UserService.Domain.ValueObjects;

namespace UserService.Application.Users.DTOs;

public class CustomerResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsStudent { get; set; } = false;
    public bool IsActive {get; set;} = false;
}
