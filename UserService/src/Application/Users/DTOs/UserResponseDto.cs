using UserService.Domain.ValueObjects;

namespace UserService.Application.Users.DTOs;

public class UserResponseDto
{
    public FullName Name { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public bool IsStudent { get; set; } = false;
}
