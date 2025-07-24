using Microsoft.Extensions.Logging;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.Services;
using NotificationService.Application.Common.Models;

namespace NotificationService.Application.Mails.Queries.SendWelcome;

public record SendWelcomeQuery : IRequest<Unit>
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
}

public class SendWelcomeQueryValidator : AbstractValidator<SendWelcomeQuery>
{
    public SendWelcomeQueryValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}

public class SendWelcomeQueryHandler : IRequestHandler<SendWelcomeQuery, Unit>
{
    private readonly ILogger<SendWelcomeQueryHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly IUserEmailBuilder _userEmailBuilder;
    
    public SendWelcomeQueryHandler(ILogger<SendWelcomeQueryHandler> logger, IEmailService emailService, IUserEmailBuilder userEmailBuilder)
    {
        _logger = logger;
        _emailService = emailService;
        _userEmailBuilder = userEmailBuilder;
    }

    public async Task<Unit> Handle(SendWelcomeQuery request, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync(new MailData()
        {
            EmailBody = await _userEmailBuilder.GenerateWelcomeTemplate(request.FirstName, request.LastName),
            EmailSubject = "Welcome to Metro Ticketing System",
            EmailToId = request.Email,
            EmailToName = request.FirstName
        });
        
        return Unit.Value;
    }
}
