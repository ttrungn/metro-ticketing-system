using UserService.Application.Users.DTOs;

namespace UserService.Application.Common.Interfaces.Services;

public interface ICustomerService
{
    Task<CustomerReadModel?> GetCustomerById(string userId);
}
