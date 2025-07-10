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
    public async Task<CustomerReadModel?> GetCustomerById(string userId)
    {
        var session = _unitOfWork.GetDocumentSession();
        var customer = await session.LoadAsync<CustomerReadModel>(userId);

        if (customer == null) return null;

        return customer;
    }
}
