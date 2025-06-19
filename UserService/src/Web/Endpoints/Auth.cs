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

    private static async Task<IResult> LoginAsync(
        [FromRoute] string role,
        [FromBody] LoginUserCommand request,
        ISender sender)
    {
        request.Role = role;
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
    
    private static async Task<IResult> RegisterAsync(
        [FromRoute] string role,
        [FromBody] RegisterUserCommand request,
        ISender sender)
    {
        request.Role = role;
        var result = await sender.Send(request);
        if (!result.Succeeded)
            return Results.BadRequest(result);

        return Results.Ok(new
        {
            result 
        });
    }
}
