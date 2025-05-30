using System.Security.Principal;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Models;

namespace UserService.Application.Users.Commands.LoginUser;

public record LoginUserCommand : IRequest<LoginUserResult>
{
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
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        (Result result, string tokenType, string token, int expiresIn) 
            = await _identityService.LoginUserAsync(request.Email, request.Password);

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
