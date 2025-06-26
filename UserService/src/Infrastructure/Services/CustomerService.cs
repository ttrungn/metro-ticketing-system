using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.DTOs;
using UserService.Infrastructure.Services.Identity;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    public async Task<CustomerResponseDto?> GetCustomerById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = await customerRepo.Query().FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        
        if(user == null || customer == null) return null;
        
        var respoone = new CustomerResponseDto()
        {
            CustomerId = customer?.Id.ToString(),
            Name = user.FullName.ToString()!,
            Email = user.Email,
            IsStudent = customer!.IsStudent,
        };
        return respoone;
    }
}
