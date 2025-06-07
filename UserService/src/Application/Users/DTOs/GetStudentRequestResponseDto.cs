namespace UserService.Application.Users.DTOs;

public class GetStudentRequestResponseDto
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public IEnumerable<StudentRequestResponseDto> Students { get; set; } = new List<StudentRequestResponseDto>();
}
