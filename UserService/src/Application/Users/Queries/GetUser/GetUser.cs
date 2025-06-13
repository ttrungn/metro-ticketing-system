using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries.GetUser;

public record GetUserQuery : IRequest<ServiceResponse<UserResponseDto>>;

public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {

    }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ServiceResponse<UserResponseDto>>
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    private readonly ILogger<GetUserQueryHandler> _logger;

    public GetUserQueryHandler(IIdentityService identityService, ILogger<GetUserQueryHandler> logger, IUser user)
    {
        _identityService = identityService;
        _logger = logger;
        _user = user;
    }

    public async Task<ServiceResponse<UserResponseDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserById(_user.Id!);

        if (user == null)
        {
            _logger.LogWarning("User with ID {userId} not found", _user.Id);
            return new ServiceResponse<UserResponseDto>
            {
                Succeeded = false,
                Message = "Không tìm thấy người dùng!",
                Data = null
            };
        }

        _logger.LogInformation("User with ID {RouteId} retrieved successfully", _user.Id);
        return new ServiceResponse<UserResponseDto>
        {
            Succeeded = true,
            Message = "Lấy thông tin người dùng thành công!",
            Data = user
        };
    }
}
