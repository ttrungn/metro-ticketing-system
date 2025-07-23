using UserService.Application.Users.DTOs;
using UserService.Application.Users.Queries.GetCustomers;

namespace UserService.Application.Common.Interfaces.Services;

public interface ICustomerService
{
    Task<CustomerReadModel?> GetCustomerById(string userId);
    Task<IEnumerable<CustomerResponseDto>> GetAllCustomers(GetCustomersQuery query);
    Task DeleteCustomerById(Guid id);
    Task ActivateCustomerById(Guid id);
}
