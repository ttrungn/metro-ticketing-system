using System.Reflection;
using NotificationService.Application.Common.Exceptions;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Security;

namespace NotificationService.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUser _user;

    public AuthorizationBehaviour(IUser user)
    {
        _user = user;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Pull all [Authorize] attributes on the request
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // 1) Must be authenticated
            if (string.IsNullOrEmpty(_user.Id))
                throw new UnauthorizedAccessException();

            // 2) Role-based checks (if any roles are specified)
            var withRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));
            if (withRoles.Any())
            {
                // Flatten all roles from all attributes into a set
                var requiredRoles = withRoles
                    .SelectMany(a => a.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(r => r.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                // Check intersection with the user's roles
                if (!_user.Roles.Any(r => requiredRoles.Contains(r)))
                    throw new ForbiddenAccessException();
            }
        }

        // Authorized (or no [Authorize] on the request) → continue
        return await next();
    }
}
