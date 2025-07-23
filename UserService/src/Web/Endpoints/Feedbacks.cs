using Microsoft.AspNetCore.Mvc;
using UserService.Application.Feedbacks.Commands.CreateFeedback;
using UserService.Application.Feedbacks.Queries.GetFeedbacks;
using UserService.Application.Feedbacks.Queries.GetUserFeedback;
using UserService.Domain.Entities;

namespace UserService.Web.Endpoints;

public class Feedbacks : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateFeedbackAsync, "/")
            .MapGet(GetFeedbacksAsync, "/")
            .MapGet(GetUserFeedbackAsync, "/user-feedbacks");
    }

    private static async Task<IResult> CreateFeedbackAsync(ISender sender,
        [FromBody] CreateFeedbackCommand command)
    {
        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/user/Feedbacks/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> GetFeedbacksAsync(ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetFeedbacksQuery(), cancellationToken);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }

    private static async Task<IResult> GetUserFeedbackAsync(
        ISender sender,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 8,
        [FromQuery] string? feedbackTypeId = null!,
        [FromQuery] string? stationId = null!,
        [FromQuery] bool? status = false,
        CancellationToken cancellationToken = default)
    {

        var query = new GetUserFeedbackQuery
        {
            Page = page,
            PageSize = pageSize,
            FeedbackTypeId = feedbackTypeId,
            StationId = stationId,
            Status = status
        };

        var response = await sender.Send(query, cancellationToken);

        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
}
