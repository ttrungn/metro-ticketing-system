using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Models;

namespace UserService.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly ITokenRepository _tokenRepository;
    private readonly string _tokenType = "Bearer";

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        ITokenRepository tokenRepository)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _tokenRepository = tokenRepository;
    }

    public async Task<(Result Result, string TokenType, string Token, int ExpiresIn)> LoginUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return (Result.Failure(["Invalid email or password."]), string.Empty, string.Empty, 0);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            return (Result.Failure(["Invalid email or password."]), string.Empty, string.Empty, 0);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenRepository.GenerateJwtToken(user.Id, user.Email!, roles);
        var expiresIn = _tokenRepository.GetTokenExpirationInSeconds();
        
        return (Result.Success(), _tokenType, token, expiresIn);
    }
    
    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }
    
}
