using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.StudentRequest;
using UserService.Application.Users.DTOs;
using UserService.Application.Users.Queries;

namespace UserService.Application.Common.Interfaces.Services;

public interface IStudentRequestService
{
    Task<Guid> StudentRequestAsync(CreateStudentRequestCommand studentRequestCommand, string userId);
    
    Task<(IEnumerable<StudentRequestResponseDto>, int)> GetAsync(GetStudentRequestQuery query, CancellationToken cancellationToken);
    Task<StudentRequestResponseDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    
    Task<Guid> UpdateAsync(UpdateStudentRequestCommand command, string? userId,CancellationToken cancellationToken = default);
}
