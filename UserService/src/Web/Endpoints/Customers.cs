using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.Queries.GetCustomer;

namespace UserService.Web.Endpoints;

public class Customers : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetCustomerByUserIdAsync, "/");
    }
    
    private static async Task<IResult> GetCustomerByUserIdAsync(ISender sender)
    {
        var response = await sender.Send(new GetCustomerQuery());

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.Unauthorized();
    }
}
