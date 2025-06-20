using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Application.Users.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<ServiceResponse<string>>
{
    public string Role { get; set; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
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

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ServiceResponse<string>>
{
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;
    private readonly IUser _user; 

    public RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger, IIdentityService identityService, IUser user)
    {
        _logger = logger;
        _identityService = identityService;
        _user = user;
    }

    public async Task<ServiceResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Role == Roles.Administrator
            || (request.Role == Roles.Staff && !_user.Roles.Contains(Roles.Administrator)))
        {
            return new ServiceResponse<string>
            {
                Succeeded = false,
                Message   = "Bạn không có quyền đăng ký người dùng với vai trò này.",
                Data      = string.Empty
            };
        }
        
        _logger.LogInformation("Starting user registration for email: {Email}", request.Email);
        var serviceResponse = await _identityService.RegisterUserAsync(request.Role, request.Email, request.Password, request.FirstName, request.LastName);
        if (!serviceResponse.Succeeded)
        {
            _logger.LogWarning("User registration failed for email: {Email}. Errors: {Errors}", request.Email, serviceResponse.Message);
        }
        else
        {
            _logger.LogInformation("User registration succeeded for email: {Email} with Id: {UserId}", request.Email, serviceResponse.Data);
        }
        return serviceResponse;
    }

}
