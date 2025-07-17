using BuildingBlocks.Domain.Constants;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;

namespace UserService.Application.Users.Commands.DeleteStaffById;

[Authorize(Roles = Roles.Administrator)]
public record DeleteStaffByIdCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}

public class DeleteStaffByIdCommandValidator : AbstractValidator<DeleteStaffByIdCommand>
{
    public DeleteStaffByIdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteStaffByIdCommandHandler : IRequestHandler<DeleteStaffByIdCommand, Unit>
{
    private readonly ILogger<DeleteStaffByIdCommandHandler> _logger;
    private readonly IStaffService _staffService;

    public DeleteStaffByIdCommandHandler(ILogger<DeleteStaffByIdCommandHandler> logger, IStaffService staffService)
    {
        _logger = logger;
        _staffService = staffService;
    }

    public async Task<Unit> Handle(DeleteStaffByIdCommand request, CancellationToken cancellationToken)
    {
        await _staffService.DeleteStaffById(request.Id);
        _logger.LogInformation("Delete staff with ID {Id} successfully", request.Id);
        return Unit.Value;
    }
}
