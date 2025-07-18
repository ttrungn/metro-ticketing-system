using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries.GetStaffById;

public record GetStaffByIdQuery : IRequest<ServiceResponse<StaffResponseDto>>
{
    public Guid Id { get; set; }
}

public class GetStaffByIdQueryValidator : AbstractValidator<GetStaffByIdQuery>
{
    public GetStaffByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetStaffByIdQueryHandler : IRequestHandler<GetStaffByIdQuery, ServiceResponse<StaffResponseDto>>
{
    private readonly ILogger<GetStaffByIdQueryHandler> _logger;
    private readonly IStaffService _staffService;

    public GetStaffByIdQueryHandler(ILogger<GetStaffByIdQueryHandler> logger, IStaffService staffService, IApplicationDbContext context)
    {
        _logger = logger;
        _staffService = staffService;
    }

    public async Task<ServiceResponse<StaffResponseDto>> Handle(GetStaffByIdQuery request, CancellationToken cancellationToken)
    {
        var staff = await _staffService.GetStaffById(request.Id);
        if (staff == null)
        {
            _logger.LogWarning("Staff with ID {staffId} not found.", request.Id);
            return new ServiceResponse<StaffResponseDto>()
            {
                Succeeded = false,
                Message = "Không tìm thấy người dùng!",
                Data = null
            };
        }
        
        _logger.LogInformation("Staff with ID {staffId} retrieved successfully.", request.Id);
        return new ServiceResponse<StaffResponseDto>()
        {
            Succeeded = true,
            Message = "Lấy người dùng!",
            Data = staff
        };
    }
}
