using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries.GetCustomers;

[Authorize(Roles = Roles.Administrator)]
public record GetCustomersQuery : IRequest<ServiceResponse<IEnumerable<CustomerResponseDto>>>
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 8;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
}

public class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(0);
    }
}

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, ServiceResponse<IEnumerable<CustomerResponseDto>>>
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<GetCustomersQueryHandler> _logger;

    public GetCustomersQueryHandler(ICustomerService customerService, ILogger<GetCustomersQueryHandler> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<CustomerResponseDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get list of customers with page: {Page} - PageSize: {PageSize} - Email: {Email}", request.Page, request.PageSize, request.Email);
        var customerDtos = await _customerService.GetAllCustomers(request);
        _logger.LogInformation("Get list of customers successfully with page: {Page} - PageSize: {PageSize} - Email: {Email}", request.Page, request.PageSize, request.Email);
        return new ServiceResponse<IEnumerable<CustomerResponseDto>>()
        {
            Succeeded = true,
            Message = "Lấy danh sách khách hàng thành công!",
            Data = customerDtos
        };
    }
}
