using NotificationService.Application.Common.Interfaces.Services;

namespace NotificationService.Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
    public async Task<string> GenerateWelcomeTemplate(string firstName, string lastName)
    {
        var html = await File.ReadAllTextAsync("Templates/Welcome.html");

        html = html.Replace("{{FirstName}}", firstName)
            .Replace("{{LastName}}", lastName);

        return html;
    }
}
