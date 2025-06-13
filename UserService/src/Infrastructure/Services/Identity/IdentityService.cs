using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.RegisterCustomer;
using UserService.Application.Users.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly ITokenRepository _tokenRepository;
    private const string TokenType = "Bearer";

    public IdentityService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        ITokenRepository tokenRepository)
    {
        _unitOfWork = unitOfWork;
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
            return (Result.Failure(["Email hoặc mật khẩu không chính xác!"]), string.Empty, string.Empty, 0);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            return (Result.Failure(["Email hoặc mật khẩu không chính xác!"]), string.Empty, string.Empty, 0);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenRepository.GenerateJwtToken(user.Id, user.Email!, roles);
        var expiresIn = _tokenRepository.GetTokenExpirationInSeconds();

        return (Result.Success(), TokenType, token, expiresIn);
    }

    public async Task<ServiceResponse<string>> RegisterUserAsync(RegisterCustomerCommand registerCustomerCommand, string role)
    {
        var user = await _userManager.FindByEmailAsync(registerCustomerCommand.Email);
        if (user != null)
        {
            return new ServiceResponse<string>()
            {
                Succeeded = false, Message = "User with this email already exists.", Data = string.Empty
            };
        }

        user = new ApplicationUser()
        {
            FullName = new FullName(registerCustomerCommand.FirstName, registerCustomerCommand.LastName),
            Email = registerCustomerCommand.Email,
            UserName = registerCustomerCommand.Email,
        };

        var createUserResult = await _userManager.CreateAsync(user, registerCustomerCommand.Password);
        if (!createUserResult.Succeeded)
        {
            var errors = createUserResult.Errors.Select(e => e.Description).ToArray();
            return new ServiceResponse<string>()
            {
                Succeeded = false, Message = string.Join(", ", errors), Data = string.Empty
            };
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
        if (!addToRoleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            var errors = addToRoleResult.Errors.Select(e => e.Description).ToArray();
            return new ServiceResponse<string>()
            {
                Succeeded = false, Message = string.Join(", ", errors), Data = string.Empty
            };
        }

        try
        {
            if (role == Roles.Customer)
            {
                var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
                var customer = new Customer
                {
                    ApplicationUserId = user.Id,
                    IsStudent         = false
                };
                await customerRepo.AddAsync(customer);
            }
            else if (role == Roles.Staff)
            {
                var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
                var staff = new Staff
                {
                    ApplicationUserId = user.Id
                };
                await staffRepo.AddAsync(staff);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _userManager.DeleteAsync(user);

            return new ServiceResponse<string>
            {
                Succeeded = false,
                Message   = "Failed to create associated Customer/Staff record: " + ex.Message,
                Data      = string.Empty
            };
        }

        return new ServiceResponse<string>()
        {
            Succeeded = true, Message = "Successfully registered", Data = user.Id
        };
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

    public async Task<UserResponseDto?> GetUserById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();

        var customer = await customerRepo.Query().FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        if (user == null) return null;

        var response = new UserResponseDto()
        {
            Name = user.FullName,
            Email = user.Email,
            IsStudent = customer?.IsStudent ?? false
        };

        return response;
    }
}
