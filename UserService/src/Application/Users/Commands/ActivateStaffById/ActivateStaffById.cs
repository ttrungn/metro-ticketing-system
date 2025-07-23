using BuildingBlocks.Domain.Constants;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;

namespace UserService.Application.Users.Commands.ActivateStaffById;

[Authorize(Roles = Roles.Administrator)]
public record ActivateStaffByIdCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}

public class ActivateStaffByIdCommandValidator : AbstractValidator<ActivateStaffByIdCommand>
{
    public ActivateStaffByIdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class ActivateStaffByIdCommandHandler : IRequestHandler<ActivateStaffByIdCommand, Unit>
{
    private readonly ILogger<ActivateStaffByIdCommandHandler> _logger;
    private readonly IStaffService _staffService;

    public ActivateStaffByIdCommandHandler(ILogger<ActivateStaffByIdCommandHandler> logger, IStaffService staffService)
    {
        _logger = logger;
        _staffService = staffService;
    }

    public async Task<Unit> Handle(ActivateStaffByIdCommand request, CancellationToken cancellationToken)
    {
        await _staffService.ActivateStaffById(request.Id);
        _logger.LogInformation("Delete staff with ID {Id} successfully", request.Id);
        return Unit.Value;
    }
}
