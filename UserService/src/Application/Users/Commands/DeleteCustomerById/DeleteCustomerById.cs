using BuildingBlocks.Domain.Constants;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;

namespace UserService.Application.Users.Commands.DeleteCustomerById;

[Authorize(Roles = Roles.Administrator)]
public record DeleteCustomerByIdCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}

public class DeleteCustomerByIdCommandValidator : AbstractValidator<DeleteCustomerByIdCommand>
{
    public DeleteCustomerByIdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteCustomerByIdCommandHandler : IRequestHandler<DeleteCustomerByIdCommand, Unit>
{
    private readonly ILogger<DeleteCustomerByIdCommandHandler> _logger;
    private readonly ICustomerService _customerService;

    public DeleteCustomerByIdCommandHandler(ILogger<DeleteCustomerByIdCommandHandler> logger, ICustomerService customerService)
    {
        _logger = logger;
        _customerService = customerService;
    }

    public async Task<Unit> Handle(DeleteCustomerByIdCommand request, CancellationToken cancellationToken)
    {
        await _customerService.DeleteCustomerById(request.Id);
        _logger.LogInformation("Delete customer with ID {Id} successfully", request.Id);
        return Unit.Value;
    }
}
