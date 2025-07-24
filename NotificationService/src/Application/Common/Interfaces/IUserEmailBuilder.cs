namespace NotificationService.Application.Common.Interfaces;

public interface IUserEmailBuilder
{
    Task<string> GenerateWelcomeTemplate(string firstName, string lastName);
}
