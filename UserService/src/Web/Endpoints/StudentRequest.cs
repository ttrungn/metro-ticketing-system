using Microsoft.AspNetCore.Mvc;
using UserService.Application.Users.Commands.StudentRequest;
using UserService.Application.Users.Queries;
using UserService.Domain.Enums;
using UserService.Domain.ValueObjects;

namespace UserService.Web.Endpoints;

public class StudentRequest : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet(GetStudentRequests, "/")
            .MapGet(GetStudentRequestById, "/{Id:guid}")
            .MapPut(ApproveStudentRequest, "/Approve/{Id:guid}")
            .MapPut(DeclinedStudentRequest, "/Declined/{Id:guid}")
            .MapPost(CreateStudentRequest, "/");
            
    }

    private async Task<IResult> GetStudentRequests(ISender sender,
        [FromQuery] int page = 0,
        [FromQuery] StudentRequestStatus? status = null,
        [FromQuery] string searchEmail = null!)
    {
        if (status.HasValue && !Enum.IsDefined(typeof(StudentRequestStatus), status.Value))
            return TypedResults.BadRequest(new { message = "Trạng thái yêu cầu không hợp lệ. Vui lòng kiểm tra lại." });
        
        var query = new GetStudentRequestQuery
        {
            Page = page,
            Status = status,
            SearchEmail = searchEmail
        };
        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }

        return TypedResults.BadRequest(response);
    }
    
        
    private async Task<IResult> GetStudentRequestById(
        [FromRoute] Guid id, ISender sender)
    {
        var query = new GetStudentRequestByIdQuery(id);
        
        var response = await sender.Send(query);
        if (response.Succeeded)
        {
            return TypedResults.Ok(response);
        }
        return TypedResults.NotFound(response);
    }

    
    private static async Task<IResult> CreateStudentRequest(ISender sender, HttpRequest request)
    {
        var form = await request.ReadFormAsync();
        var studentCardImage = form.Files.GetFile("studentCardImage");
        Stream? studentCardImageStream = null;
        string? studentCardImageName = null;
        
        if (studentCardImage is {Length: > 0})
        {
            studentCardImageStream = studentCardImage.OpenReadStream();
            studentCardImageName = studentCardImage.FileName;
        }
        
        var command = new CreateStudentRequestCommand
        {
            StudentCode = form["studentCode"].ToString(),
            StudentEmail = form["studentEmail"].ToString(),
            SchoolName = form["schoolName"].ToString(),
            FullName = new FullName(form["firstName"].ToString(), form["lastName"].ToString()),
            DateOfBirth = DateTimeOffset.Parse(form["dateOfBirth"]!),
            StudentCardImageStream = studentCardImageStream,
            StudentCardImageName = studentCardImageName
        };
        
        var response = await sender.Send(command);

        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/user/StudentRequest/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }
    private static async Task<IResult> ApproveStudentRequest(
        [FromRoute] Guid id, 
        ISender sender)
    {
        var command = new UpdateStudentRequestCommand
        {
            Id = id,
            Status = StudentRequestStatus.Approved
        };
        var response = await sender.Send(command);
        
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/user/StudentRequest/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }
    private static async Task<IResult> DeclinedStudentRequest(
        [FromRoute] Guid id, 
        ISender sender)
    {
        var command = new UpdateStudentRequestCommand
        {
            Id = id,
            Status = StudentRequestStatus.Declined
        };
        var response = await sender.Send(command);
        
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/user/StudentRequest/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }
}
