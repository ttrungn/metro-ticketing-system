using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces;

namespace NotificationService.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUser _user;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user)
    {
        _logger = logger;
        _user = user;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        _logger.LogInformation("NotificationService Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        return Task.CompletedTask;
    }
}
