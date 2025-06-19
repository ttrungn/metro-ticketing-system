using BuildingBlocks.Response;
using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.RegisterUser;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Common.Interfaces.Services;

public interface IIdentityService
{
    Task<(Result Result, string TokenType, string Token, int ExpiresIn)> LoginUserAsync(string role, string email, string password);

    Task<ServiceResponse<string>> RegisterUserAsync(string role, string email, string password, string firstName, string lastName);
    
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);
    
    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<UserResponseDto?> GetUserById(string userId);
}
