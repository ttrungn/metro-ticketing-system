using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using SampleService.Application.Common.Interfaces;

namespace SampleService.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Name);

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)
        .Select(r => r.Value) ?? [];
}
