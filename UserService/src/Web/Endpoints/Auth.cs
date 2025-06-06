using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.LoginUser;
using UserService.Application.Users.Commands.RegisterCustomer;

namespace UserService.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapPost(LoginAsync, "/login")
            .MapPost(RegisterCustomerAsync, "/register/customer");
    }

    private static async Task<IResult> LoginAsync([FromBody] LoginUserCommand request, ISender sender)
    {
        var result = await sender.Send(request);
        return !result.Succeeded ? Results.BadRequest(new {message = result.Errors.First()}) : 
            Results.Ok(new { result.TokenType, result.Token, result.ExpiresIn });
    }
    
    private static async Task<IResult> RegisterCustomerAsync([FromBody] RegisterCustomerCommand request, ISender sender)
    {
        var result = await sender.Send(request);
        return !result.Succeeded ? Results.BadRequest(result) : Results.Ok(result);
    }
}
