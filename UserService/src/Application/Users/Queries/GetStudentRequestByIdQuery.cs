using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries;
// [Authorize(Roles = Roles.Staff)]
public record GetStudentRequestByIdQuery(Guid Id) : IRequest<ServiceResponse<StudentRequestResponseDto>>;

public class GetStudentRequestByIdQueryValidator : AbstractValidator<GetStudentRequestByIdQuery>
{
    public GetStudentRequestByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Request ID is required.");
    }
}

public class GetStudentRequestByIdQueryHandler : IRequestHandler<GetStudentRequestByIdQuery, ServiceResponse<StudentRequestResponseDto>>
{
    private readonly IStudentRequestService _studentRequestService;

    public GetStudentRequestByIdQueryHandler(IStudentRequestService studentRequestService)
    {
        _studentRequestService = studentRequestService;
    }

    public async Task<ServiceResponse<StudentRequestResponseDto>> Handle(GetStudentRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var studentRequest = await _studentRequestService.GetByIdAsync(request.Id, cancellationToken);
        if (studentRequest == null)
        {
            return new ServiceResponse<StudentRequestResponseDto>
            {
                Succeeded = false,
                Message = "Student request not found."
            };
        }

        return new ServiceResponse<StudentRequestResponseDto>
        {
            Succeeded = true,
            Message = "Student request retrieved successfully.",
            Data = studentRequest
        };
    }
}

