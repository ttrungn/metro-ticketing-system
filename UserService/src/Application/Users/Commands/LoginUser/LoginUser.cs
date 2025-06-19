using System.Security.Principal;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Models;

namespace UserService.Application.Users.Commands.LoginUser;

public record LoginUserCommand : IRequest<LoginUserResult>
{
    public string Role { get; set; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}

public class LoginUserResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = [];
    public string TokenType { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }      
}

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly ILogger<LoginUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(ILogger<LoginUserCommandHandler> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt started for Email: {Email}", request.Email);
        (Result result, string tokenType, string token, int expiresIn)
            = await _identityService.LoginUserAsync(request.Role, request.Email, request.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("Login successful for Email: {Email}", request.Email);
        }
        else
        {
            _logger.LogWarning("Login failed for Email: {Email}. Errors: {Errors}", request.Email, string.Join(", ", result.Errors));
        }

        return new LoginUserResult()
        {
            Succeeded = result.Succeeded,
            Errors    = result.Errors,
            TokenType = tokenType,
            Token     = token,
            ExpiresIn = expiresIn
        };
    }
}
