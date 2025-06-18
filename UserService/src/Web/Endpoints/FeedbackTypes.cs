using Microsoft.AspNetCore.Mvc;
using UserService.Application.Feedbacks.Commands.CreateFeedbackType;
using UserService.Application.Feedbacks.Queries.GetFeedbackTypes;

namespace UserService.Web.Endpoints;

public class FeedbackTypes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateFeedbackTypeAsync, "/")
            .MapGet(GetFeedbackTypesAsync, "/");
    }

    private static async Task<IResult> CreateFeedbackTypeAsync(ISender sender, [FromBody] CreateFeedbackTypeCommand command)
    {
        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/user/FeedbackTypes/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> GetFeedbackTypesAsync(ISender sender)
    {
        var response = await sender.Send(new GetFeedbackTypesQuery());

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
}
