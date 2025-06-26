using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries.GetCustomer;

public record GetCustomerQuery : IRequest<ServiceResponse<CustomerResponseDto>>;

public class GetCustomerQueryValidator : AbstractValidator<GetCustomerQuery>
{
    public GetCustomerQueryValidator()
    {
    }
}

public class GetCustomerHandler : IRequestHandler<GetCustomerQuery, ServiceResponse<CustomerResponseDto>>
{
    private readonly ICustomerService _customerService;
    private readonly IUser _user;
    private readonly ILogger<GetCustomerHandler> _logger;
    
    public GetCustomerHandler(ICustomerService customerService, IUser user, ILogger<GetCustomerHandler> logger)
    {
        _customerService = customerService;
        _user = user;
        _logger = logger;
    }

    public async Task<ServiceResponse<CustomerResponseDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetCustomerById(_user.Id!);

        if (customer == null)
        {
            _logger.LogWarning("User with ID {userId} not found", _user.Id);
            return new ServiceResponse<CustomerResponseDto>
            {
                Succeeded = false,
                Message = "Không tìm thấy khách hàng!",
                Data = null
            };
        }
        _logger.LogInformation("User with ID {RouteId} retrieved successfully", _user.Id);
        return new ServiceResponse<CustomerResponseDto>
        {
            Succeeded = true,
            Message = "Lấy thông tin khách hàng thành công!",
            Data = customer
        };
    }
}
