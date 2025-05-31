using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Users.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<RegisterUserResult>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
}

public class RegisterUserResult : HandlerResult
{
    public string? UserId { get; set; }
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(5, 12).WithMessage("Password must be between 5 and 12 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting user registration for email: {Email}", request.Email);

        var (result, userId) = await _identityService.RegisterUserAsync(request, Roles.Customer);

        if (!result.Succeeded)
        {
            _logger.LogWarning("User registration failed for email: {Email}. Errors: {Errors}",
                request.Email, string.Join(", ", result.Errors));

            return new RegisterUserResult()
            {
                Status = "failed",
                Message = result.Errors.FirstOrDefault() ?? "Registration failed.",
                UserId = null
            };
        }

        _logger.LogInformation("User registration succeeded for email: {Email} with Id: {UserId}", request.Email, userId);

        return new RegisterUserResult()
        {
            Status = "success",
            Message = "Successfully registered",
            UserId = userId
        };
    }

}
