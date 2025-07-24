using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.Services;

namespace NotificationService.Infrastructure.Services.Email;

public class UserEmailBuilder : IUserEmailBuilder
{
    public async Task<string> GenerateWelcomeTemplate(string firstName, string lastName)
    {
        var html = await File.ReadAllTextAsync("Templates/Welcome.html");

        html = html.Replace("{{FirstName}}", firstName)
            .Replace("{{LastName}}", lastName);

        return html;
    }
}
