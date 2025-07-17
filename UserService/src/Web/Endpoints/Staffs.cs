using Microsoft.AspNetCore.Mvc;
using UserService.Application.Users.Commands.DeleteStaffById;
using UserService.Application.Users.Commands.UpdateStaffById;
using UserService.Application.Users.Queries.GetStaffById;
using UserService.Application.Users.Queries.GetStaffs;

namespace UserService.Web.Endpoints;

public class Staffs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet(GetStaffsAsync, "/")
            .MapGet(GetStaffByIdAsync, "/{id:guid}")
            .MapPut(UpdateStaffByIdAsync, "/{id:guid}")
            .MapDelete(DeleteStaffByIdAsync, "/{id:guid}");
    }

    private static async Task<IResult> GetStaffByIdAsync(ISender sender, [FromRoute] Guid id)
    {
        var response = await sender.Send(new GetStaffByIdQuery()
        {
            Id = id
        });
        if (!response.Succeeded)
        {
            return TypedResults.NotFound(response);
        }
        
        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetStaffsAsync(
        ISender sender,
        [FromQuery] int page = 0, 
        [FromQuery] int pageSize = 8, 
        [FromQuery] string email = "",
        [FromQuery] bool isActive = true)
    {
        var response = await sender.Send(new GetStaffsQuery()
        {
            Page = page,
            PageSize = pageSize,
            Email = email,
            IsActive = isActive
        });
        
        return TypedResults.Ok(response);
    }
    
    private class StaffUpdateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
    
    private static async Task<IResult> UpdateStaffByIdAsync(
        ISender sender, 
        [FromRoute] Guid id,
        [FromBody] StaffUpdateDto staff
        )
    {
        var response = await sender.Send(new UpdateStaffByIdCommand()
        {
            Id = id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email
        });
        if (!response.Succeeded)
        {
            return TypedResults.NotFound(response);
        }
        
        return TypedResults.NoContent();
    }
    
    private static async Task<IResult> DeleteStaffByIdAsync(ISender sender, [FromRoute] Guid id)
    {
        var response = await sender.Send(new DeleteStaffByIdCommand()
        {
            Id = id
        });
        
        return TypedResults.NoContent();
    }
}
