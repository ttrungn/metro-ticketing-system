using NotificationService.Application.Mails.Queries.SendWelcome;

namespace NotificationService.Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task<bool> SendMailAsync(MailData mailData);
}
