using BuildingBlocks.Response;
using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.RegisterCustomer;

namespace UserService.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<(Result Result, string TokenType, string Token, int ExpiresIn)> LoginUserAsync(string email, string password);

    Task<ServiceResponse<string>> RegisterUserAsync(RegisterCustomerCommand registerCustomerCommand, string role);
    
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);
}
