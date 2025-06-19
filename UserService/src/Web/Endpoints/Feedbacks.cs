using Microsoft.AspNetCore.Mvc;
using UserService.Application.Feedbacks.Commands.CreateFeedback;
using UserService.Application.Feedbacks.Queries.GetFeedbacks;

namespace UserService.Web.Endpoints;

public class Feedbacks : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateFeedbackAsync, "/")
            .MapGet(GetFeedbacksAsync, "/");
    }

    private static async Task<IResult> CreateFeedbackAsync(ISender sender, [FromBody] CreateFeedbackCommand command)
    {
        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/user/Feedbacks/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> GetFeedbacksAsync(ISender sender, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetFeedbacksQuery(), cancellationToken);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
}
