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
    public int CurrentPage { get; init; } = 0;
    public int PageSize { get; init; } = 8;
    public StudentRequestStatus? Status { get; init; } = null;
    public string SearchEmail { get; init; } = null!;
}

public class GetStudentRequestQueryValidator : AbstractValidator<GetStudentRequestQuery>
{
    public GetStudentRequestQueryValidator()
    {
        RuleFor(x => x.CurrentPage)
            .GreaterThanOrEqualTo(0).WithMessage("Trang hiện tại phải lớn hơn hoặc bằng 0!");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Kích thước trang phải lớn hơn 0!");
    }
}
public class GetStudentRequestQueryHandler : IRequestHandler<GetStudentRequestQuery, 
                                                            ServiceResponse<GetStudentRequestResponseDto>>
{
    private readonly IStudentRequestService _studentRequestService;
    
    public GetStudentRequestQueryHandler(IStudentRequestService studentRequestService)
    {
        _studentRequestService = studentRequestService;
    }

    public async Task<ServiceResponse<GetStudentRequestResponseDto>> Handle(GetStudentRequestQuery request, 
                                                                            CancellationToken cancellationToken)
    {
        var (studentRequests, totalCount) = 
            await _studentRequestService.GetAsync(request, cancellationToken);
        var response = new GetStudentRequestResponseDto
        {
            Students = studentRequests,
            TotalPages = totalCount,
            CurrentPage = request.CurrentPage,
            PageSize = request.PageSize
        };
        return new ServiceResponse<GetStudentRequestResponseDto>
        {
            Succeeded = true,
            Data = response,
            Message = "Student requests retrieved successfully."
        };
    }
}
