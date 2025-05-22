using SampleService.Application.Common.Interfaces;

namespace SampleService.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();

    public string? Email =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-User-Email"].FirstOrDefault();

    public string? UserName =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-User-Name"].FirstOrDefault();

    public IEnumerable<string> Roles =>
        (_httpContextAccessor.HttpContext?.Request.Headers["X-User-Roles"]
            .FirstOrDefault() ?? "")
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(r => r.Trim());
}
