using UserService.Application.Users.Queries.GetUser;

namespace UserService.Web.Endpoints;

public class Profiles : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(GetUserByEmailAsync, "/");
    }

    private static async Task<IResult> GetUserByEmailAsync(GetUserQuery query, ISender sender)
    {
        var response = await sender.Send(query);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.Unauthorized();
    }
}
