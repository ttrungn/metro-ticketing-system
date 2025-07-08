using BuildingBlocks.Domain.Events.Users;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Users.DTOs;
using UserService.Domain.Enums;
using UserService.Domain.ValueObjects;

namespace UserService.Web.Consumers.StudentRequest;

public class CreateStudentRequestConsumer : IConsumer<CreateStudentRequestEvent>
{
    private readonly ILogger<CreateStudentRequestConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentRequestConsumer(ILogger<CreateStudentRequestConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task Consume(ConsumeContext<CreateStudentRequestEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateStudentRequestConsumer Message Received: {StudentRequestId}", message.Id);
        var session = _unitOfWork.GetDocumentSession();
        var studentRequestReadModel = new StudentRqReadModel()
        {
            Id = message.Id,
            CustomerId = message.CustomerId,
            StaffId = message.StaffId,
            StudentCode = message.StudentCode,
            StudentEmail = message.StudentEmail,
            SchoolName = message.SchoolName,
            FullName = new FullName(message.FirstName, message.LastName),
            DateOfBirth = message.DateOfBirth,
            StudentCardImageUrl = message.StudentCardImageUrl,
            Status = StudentRequestStatus.Pending,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag
        };
        session.Store(studentRequestReadModel);
        await session.SaveChangesAsync();
         
    }
}
