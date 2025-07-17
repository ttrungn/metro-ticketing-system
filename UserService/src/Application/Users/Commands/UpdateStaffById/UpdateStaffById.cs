using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;

namespace UserService.Application.Users.Commands.UpdateStaffById;

[Authorize(Roles = Roles.Administrator)]
public record UpdateStaffByIdCommand : IRequest<ServiceResponse<bool>>
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
}

public class UpdateStaffByIdCommandValidator : AbstractValidator<UpdateStaffByIdCommand>
{
    public UpdateStaffByIdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}

public class UpdateStaffByIdCommandHandler : IRequestHandler<UpdateStaffByIdCommand, ServiceResponse<bool>>
{
    private readonly ILogger<UpdateStaffByIdCommandHandler> _logger;
    private readonly IStaffService _staffService;
    
    public UpdateStaffByIdCommandHandler(ILogger<UpdateStaffByIdCommandHandler> logger, IStaffService staffService)
    {
        _logger = logger;
        _staffService = staffService;
    }

    public async Task<ServiceResponse<bool>> Handle(UpdateStaffByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _staffService.UpdateStaffById(request);
        if (!result)
        {
            _logger.LogWarning("Staff with ID {staffId} not found.", request.Id);
            return new ServiceResponse<bool>()
            {
                Succeeded = false,
                Message = "Không tìm thấy người dùng!",
                Data = false
            };
        }
        
        _logger.LogInformation("Staff with ID {staffId} updated successfully.", request.Id);
        return new ServiceResponse<bool>()
        {
            Succeeded = true,
            Message = "Update staff successfully!",
            Data = result
        };
    }
}
