using UserService.Application.Users.Queries.GetUser;

namespace UserService.Web.Endpoints;

public class Profiles : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetUserByEmailAsync, "/");
    }

    private static async Task<IResult> GetUserByEmailAsync(ISender sender)
    {
        var response = await sender.Send(new GetUserQuery());

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.Unauthorized();
    }
}
