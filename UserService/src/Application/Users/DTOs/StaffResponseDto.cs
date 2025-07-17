namespace UserService.Application.Users.DTOs;

public class StaffResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive {get; set;} = false;
}
