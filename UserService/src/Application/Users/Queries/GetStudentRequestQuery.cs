using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;
using UserService.Application.Users.DTOs;
using UserService.Domain.Enums;

namespace UserService.Application.Users.Queries;
// [Authorize(Roles = Roles.Staff)]
public record GetStudentRequestQuery : IRequest<ServiceResponse<GetStudentRequestResponseDto>>
{
    public int Page { get; init; } = 0;
    public StudentRequestStatus? Status { get; init; } = null;
    public string SearchEmail { get; init; } = null!;
}

public class GetStudentRequestQueryHandler : IRequestHandler<GetStudentRequestQuery, 
                                                            ServiceResponse<GetStudentRequestResponseDto>>
{
    private readonly IStudentRequestService _studentRequestService;
    private const int DefaultPageSize = 8;
    
    public GetStudentRequestQueryHandler(IStudentRequestService studentRequestService)
    {
        _studentRequestService = studentRequestService;
    }

    public async Task<ServiceResponse<GetStudentRequestResponseDto>> Handle(GetStudentRequestQuery request, 
                                                                            CancellationToken cancellationToken)
    {
        var (studentRequests, totalCount) = 
            await _studentRequestService.GetAsync(request, DefaultPageSize, cancellationToken);
        var response = new GetStudentRequestResponseDto
        {
            Students = studentRequests,
            TotalPages = totalCount,
            CurrentPage = request.Page,
        };
        return new ServiceResponse<GetStudentRequestResponseDto>
        {
            Succeeded = true,
            Data = response,
            Message = "Student requests retrieved successfully."
        };
    }
}
