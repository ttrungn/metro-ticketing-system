using UserService.Infrastructure.Identity;

namespace UserService.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this);
    }
}
