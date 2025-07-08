namespace UserService.Application.Users.DTOs;

public class GetStudentRequestResponseDto
{
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 8;
    public int CurrentPage { get; set; } = 0;
    public IEnumerable<StudentRqReadModel> Students { get; set; } = new List<StudentRqReadModel>();
}
