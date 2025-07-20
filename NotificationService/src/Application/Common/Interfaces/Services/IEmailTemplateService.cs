namespace NotificationService.Application.Common.Interfaces.Services;

public interface IEmailTemplateService
{
    Task<string> GenerateWelcomeTemplate(string firstName, string lastName);
}
