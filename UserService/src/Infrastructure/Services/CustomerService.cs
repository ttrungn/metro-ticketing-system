using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.DTOs;
using UserService.Application.Users.Queries.GetCustomers;
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

    public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomers(GetCustomersQuery query)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customerResponseDtos = await customerRepo.Query()
            .Join(
                _userManager.Users, 
                c => c.ApplicationUserId, 
                u => u.Id, 
                (c, u) => new CustomerResponseDto()
                {
                    Id = c.Id,
                    Email = u.Email!,
                    Name = $"{u.FullName.FirstName} {u.FullName.LastName}",
                    IsStudent = c.IsStudent,
                    IsActive = !c.DeleteFlag
                })
            .Where(c=> c.Email.Contains(query.Email.Trim()) && c.IsActive == query.IsActive)
            .Skip(query.Page * query.PageSize)
            .Take(query.PageSize)
            .OrderBy(c => c.Email)
            .ToListAsync();
        
        return customerResponseDtos;
    }

    public async Task DeleteCustomerById(Guid id)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = customerRepo.Query().FirstOrDefault(c => c.Id == id);
        if (customer == null) return;
        
        customer.DeleteFlag = true;
        
        await customerRepo.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task ActivateCustomerById(Guid id)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = customerRepo.Query().FirstOrDefault(c => c.Id == id);
        if (customer == null) return;
        
        customer.DeleteFlag = false;
        
        await customerRepo.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync();
    }
}
