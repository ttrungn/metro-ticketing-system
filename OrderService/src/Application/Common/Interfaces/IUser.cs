namespace OrderService.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    string? UserName { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
}
