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
            .MapPut(UpdateStudentRequest, "/{Id:guid}")
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

    
    private static async Task<IResult> CreateStudentRequest(
        [FromForm] IFormFile? file, 
        [FromQuery] string studentCode, 
        [FromQuery] string studentEmail, 
        [FromQuery] string firstName,
        [FromQuery] string lastName,
        [FromQuery] DateTimeOffset dateOfBirth, 
        ISender sender)
    {
        if (file == null)
        {
            return TypedResults.BadRequest(new { message = "Vui lòng tải lên ảnh thẻ sinh viên." });
        }
        var request = new CreateStudentRequestCommand
        {
            StudentCode = studentCode,
            StudentEmail = studentEmail,
            FullName = new FullName(firstName, lastName),
            DateOfBirth = dateOfBirth,
            StudentCardImageUrl = file.FileName 
        };

        var response = await sender.Send(request);

        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/user/StudentRequest/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }
    private static async Task<IResult> UpdateStudentRequest(
        [FromRoute] Guid id, 
        [FromQuery] StudentRequestStatus status,
        ISender sender)
    {
        var command = new UpdateStudentRequestCommand
        {
            Id = id,
            Status = status
        };
        var response = await sender.Send(command);
        
        if (response.Succeeded)
        {
            return TypedResults.Created($"/api/user/StudentRequest/{response.Data}", response);
        }

        return TypedResults.BadRequest(response);
    }
}
