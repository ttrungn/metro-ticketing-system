using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.StudentRequest;
using UserService.Application.Users.DTOs;
using UserService.Application.Users.Queries;

namespace UserService.Application.Common.Interfaces.Services;

public interface IStudentRequestService
{
    Task<Guid> CreateStudentRequestAsync(CreateStudentRequestCommand studentRequestCommand, string userId);
    
    Task<(IEnumerable<StudentRqReadModel>, int)> GetAsync(GetStudentRequestQuery query, CancellationToken cancellationToken);
    Task<StudentRqReadModel?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    
    Task<Guid> UpdateAsync(UpdateStudentRequestCommand command, string? userId,CancellationToken cancellationToken = default);
}
