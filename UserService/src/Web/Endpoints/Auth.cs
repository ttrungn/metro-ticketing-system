using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.LoginUser;

namespace UserService.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(LoginAsync, "/login");
    }

    private static async Task<IResult> LoginAsync([FromBody] LoginUserCommand request, ISender sender)
    {
        var result = await sender.Send(request);
        return !result.Succeeded ? Results.BadRequest(new {message = result.Errors.First()}) : 
            Results.Ok(new { result.TokenType, result.Token, result.ExpiresIn });
    }
}
