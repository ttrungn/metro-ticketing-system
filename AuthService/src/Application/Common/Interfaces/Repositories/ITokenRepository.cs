namespace AuthService.Application.Common.Interfaces.Repositories;

public interface ITokenRepository
{
    string GenerateJwtToken(string userId, string userEmail, IEnumerable<string> roles);
}
