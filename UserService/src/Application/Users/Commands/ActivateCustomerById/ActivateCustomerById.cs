using BuildingBlocks.Domain.Constants;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;

namespace UserService.Application.Users.Commands.ActivateCustomerById;

[Authorize(Roles = Roles.Administrator)]
public record ActivateCustomerByIdCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}

public class ActivateCustomerByIdCommandValidator : AbstractValidator<ActivateCustomerByIdCommand>
{
    public ActivateCustomerByIdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class ActivateCustomerByIdCommandHandler : IRequestHandler<ActivateCustomerByIdCommand, Unit>
{
    private readonly ILogger<ActivateCustomerByIdCommandHandler> _logger;
    private readonly ICustomerService _customerService;

    public ActivateCustomerByIdCommandHandler(ILogger<ActivateCustomerByIdCommandHandler> logger, ICustomerService customerService)
    {
        _logger = logger;
        _customerService = customerService;
    }

    public async Task<Unit> Handle(ActivateCustomerByIdCommand request, CancellationToken cancellationToken)
    {
        await _customerService.ActivateCustomerById(request.Id);
        _logger.LogInformation("Delete customer with ID {Id} successfully", request.Id);
        return Unit.Value;
    }
}
