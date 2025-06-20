using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.LoginUser;
using UserService.Application.Users.Commands.RegisterUser;

namespace UserService.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(LoginAsync, "/{role}/login")
            .MapPost(RegisterAsync, "/{role}/register");
    }
    
    public class LoginRequest
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
    
    private static async Task<IResult> LoginAsync(
        [FromRoute] string role,
        [FromBody] LoginRequest loginRequest,
        ISender sender)
    {
        var request = new LoginUserCommand
        {
            Email = loginRequest.Email,
            Password = loginRequest.Password,
            Role = role
        };
        var result = await sender.Send(request);
        if (!result.Succeeded)
            return Results.BadRequest(new { message = result.Errors.First() });

        return Results.Ok(new
        {
            result.TokenType,
            result.Token,
            result.ExpiresIn
        });
    }
    
    public class RegisterRequest
    {
        public string Email     { get; init; } = null!;
        public string Password  { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName  { get; init; } = null!;
    }
    
    private static async Task<IResult> RegisterAsync(
        [FromRoute] string role,
        [FromBody] RegisterRequest registerRequest,
        ISender sender)
    {
        var request = new RegisterUserCommand
        {
            Email     = registerRequest.Email,
            Password  = registerRequest.Password,
            FirstName = registerRequest.FirstName,
            LastName  = registerRequest.LastName,
            Role      = role
        };
        var result = await sender.Send(request);
        if (!result.Succeeded)
            return Results.BadRequest(result);

        return Results.Ok(new
        {
            result 
        });
    }
}
