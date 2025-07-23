using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.Commands.ActivateCustomerById;
using UserService.Application.Users.Commands.ActivateStaffById;
using UserService.Application.Users.Commands.DeleteCustomerById;
using UserService.Application.Users.Queries.GetCustomer;
using UserService.Application.Users.Queries.GetCustomers;

namespace UserService.Web.Endpoints;

public class Customers : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet(GetCustomerByUserIdAsync, "/profile")
            .MapGet(GetCustomersAsync, "/")
            .MapDelete(DeleteCustomerByIdAsync, "/deactivate/{id:guid}")
            .MapPut(ActivateCustomerByIdAsync, "/activate/{id:guid}");
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
    
    private static async Task<IResult> GetCustomersAsync(
        ISender sender,
        [FromQuery] int page = 0, 
        [FromQuery] int pageSize = 8,
        [FromQuery] bool isActive = true,
        [FromQuery] string email = "" 
        )
    {
        var query = new GetCustomersQuery()
        {
            Page = page,
            PageSize = pageSize,
            Email = email,
            IsActive = isActive
        };
        
        var response = await sender.Send(query);

        if (!response.Succeeded)
        {
            return TypedResults.BadRequest(response);
        }

        return TypedResults.Ok(response);
    }
    
    private static async Task<IResult> DeleteCustomerByIdAsync(ISender sender, [FromRoute] Guid id)
    {
        var query = new DeleteCustomerByIdCommand(){Id = id};

        var response = await sender.Send(query);

        return TypedResults.NoContent();
    }
    
    private static async Task<IResult> ActivateCustomerByIdAsync(ISender sender, [FromRoute] Guid id)
    {
        var query = new ActivateCustomerByIdCommand()
        {
            Id = id
        };

        var response = await sender.Send(query);

        return TypedResults.NoContent();
    }
}
