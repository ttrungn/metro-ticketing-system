using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.RegisterUser;

namespace UserService.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<(Result Result, string TokenType, string Token, int ExpiresIn)> LoginUserAsync(string email, string password);

    Task<(Result Result, string? Id)> RegisterUserAsync(RegisterUserCommand registerUserCommand, string role);
    
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);
}
