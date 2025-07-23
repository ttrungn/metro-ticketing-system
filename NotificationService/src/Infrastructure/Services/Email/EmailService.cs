using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Application.Common.Interfaces.Services;
using NotificationService.Application.Common.Models;
using NotificationService.Application.Mails.Queries.SendWelcome;

namespace NotificationService.Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly MailSettings _mailSettings;

        public EmailService(ILogger<EmailService> logger, IOptions<MailSettings> options)
        {
            _logger = logger;
            _mailSettings = options.Value;
        }

        public async Task<bool> SendMailAsync(MailData mailData)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(_mailSettings.Name, _mailSettings.EmailId));
                emailMessage.To.Add(new MailboxAddress(mailData.EmailToName, mailData.EmailToId));
                emailMessage.Subject = mailData.EmailSubject;

                var builder = new BodyBuilder
                {
                    HtmlBody = mailData.EmailBody
                };

                emailMessage.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSSL ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                await smtp.SendAsync(emailMessage);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {EmailToId}", mailData.EmailToId);
                return false;
            }
        }
    }
}
