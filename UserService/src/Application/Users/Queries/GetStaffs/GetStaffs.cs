using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries.GetStaffs;

public record GetStaffsQuery : IRequest<ServiceResponse<IEnumerable<StaffResponseDto>>>
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 8;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
}

public class GetStaffsQueryValidator : AbstractValidator<GetStaffsQuery>
{
    public GetStaffsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(0);
    }
}

public class GetStaffsQueryHandler : IRequestHandler<GetStaffsQuery, ServiceResponse<IEnumerable<StaffResponseDto>>>
{
    private readonly ILogger<GetStaffsQueryHandler> _logger;
    private readonly IStaffService _staffService;

    public GetStaffsQueryHandler(ILogger<GetStaffsQueryHandler> logger, IStaffService staffService)
    {
        _logger = logger;
        _staffService = staffService;
    }

    public async Task<ServiceResponse<IEnumerable<StaffResponseDto>>> Handle(GetStaffsQuery request, CancellationToken cancellationToken)
    {
        var staffs = await _staffService.GetAllStaffs(request);
        return new ServiceResponse<IEnumerable<StaffResponseDto>>()
        {
            Succeeded = true,
            Message = "Lấy danh sách người dùng!",
            Data = staffs
        };
    }
}
