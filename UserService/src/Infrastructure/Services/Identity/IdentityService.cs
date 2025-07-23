using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Domain.Events.Users;
using BuildingBlocks.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.RegisterUser;
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

    public async Task<(Result Result, string TokenType, string Token, int ExpiresIn)> LoginUserAsync(string role, string email, string password)
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
        var hasRole = roles.Select(r => r.ToLower()).Contains(role.ToLower());
        if (!hasRole)
        {
            return (Result.Failure(["Email hoặc mật khẩu không chính xác!"]), string.Empty, string.Empty, 0);
        }

        if (role.ToLower() == Roles.Customer.ToLower())
        {
            var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
            var customer = await customerRepo.Query().FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (customer == null)
            {
                return (Result.Failure(["Email hoặc mật khẩu không chính xác!"]), string.Empty, string.Empty, 0);
            }
            if (customer.DeleteFlag)
            {
                return (Result.Failure(["Tài khoản bị khóa, vui lòng liên hệ với hệ thống để được hỗ trợ!"]), string.Empty, string.Empty, 0);
            }
        } else if (role.ToLower() == Roles.Staff.ToLower())
        {
            var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
            var staff = await staffRepo.Query().FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (staff == null)
            {
                return (Result.Failure(["Email hoặc mật khẩu không chính xác!"]), string.Empty, string.Empty, 0);
            }
            if (staff.DeleteFlag)
            {
                return (Result.Failure(["Tài khoản bị khóa, vui lòng liên hệ với hệ thống để được hỗ trợ!"]), string.Empty, string.Empty, 0);
            }
        }

        var token = _tokenRepository.GenerateJwtToken(user.Id, user.Email!, roles);
        var expiresIn = _tokenRepository.GetTokenExpirationInSeconds();

        return (Result.Success(), TokenType, token, expiresIn);
    }

    public async Task<ServiceResponse<string>> RegisterUserAsync(string role, string email, string password, string firstName, string lastName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return new ServiceResponse<string>
            {
                Succeeded = false,
                Message = "Email này đã được sử dụng.",
                Data = string.Empty
            };
        }

        user = new ApplicationUser
        {
            FullName = new FullName(firstName, lastName),
            Email = email,
            UserName = email,
        };

        var createUserResult = await _userManager.CreateAsync(user, password);
        if (!createUserResult.Succeeded)
        {
            return new ServiceResponse<string>
            {
                Succeeded = false,
                Message = "Không thể tạo tài khoản. Vui lòng thử lại.",
                Data = string.Empty
            };
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
        if (!addToRoleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return new ServiceResponse<string>
            {
                Succeeded = false,
                Message = "Không thể tạo tài khoản. Vui lòng thử lại.",
                Data = string.Empty
            };
        }

        try
        {
            if (role.ToLower() == Roles.Customer.ToLower())
            {
                var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
                var customer = new Customer()
                {
                    ApplicationUserId = user.Id,
                    IsStudent = false
                };
                await customerRepo.AddAsync(customer);
                customer.AddDomainEvent(new CreateCustomerEvent()
                {
                    ApplicationUserId = customer.ApplicationUserId,
                    Email = user.Email,
                    FirstName = user.FullName.FirstName,
                    LastName = user.FullName.LastName,
                    CustomerId = customer.Id,
                    IsStudent = customer.IsStudent,
                    DeleteFlag = false
                });
            }
            else if (role.ToLower() == Roles.Staff.ToLower())
            {
                var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
                var staff = new Staff()
                {
                    ApplicationUserId = user.Id
                };
                await staffRepo.AddAsync(staff);
                staff.AddDomainEvent(new CreateStaffEvent()
                {
                    ApplicationUserId = staff.ApplicationUserId,
                    Email = user.Email,
                    FirstName = user.FullName.FirstName,
                    LastName = user.FullName.LastName,
                    StaffId = staff.Id,
                    DeleteFlag = false
                });
            }
            else
            {
                throw new Exception("Role không hợp lệ!");
            }
            await _unitOfWork.SaveChangesAsync();
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            return new ServiceResponse<string>
            {
                Succeeded = false,
                Message = "Không thể tạo tài khoản. Vui lòng thử lại.",
                Data = string.Empty
            };
        }

        return new ServiceResponse<string>
        {
            Succeeded = true,
            Message = "Đăng ký thành công",
            Data = user.Id
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

    public async Task<CustomerReadModel?> GetUserById(string userId)
    {
        var session = _unitOfWork.GetDocumentSession();
        var user = await session.LoadAsync<CustomerReadModel>(userId);

        if (user == null) return null;

        return user;
    }
}
