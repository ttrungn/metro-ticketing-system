namespace UserService.Application.Common.Interfaces.Repositories;

public interface ITokenRepository
{
    int GetTokenExpirationInSeconds();
    string GenerateJwtToken(string userId, string userEmail, IEnumerable<string> roles);
}
