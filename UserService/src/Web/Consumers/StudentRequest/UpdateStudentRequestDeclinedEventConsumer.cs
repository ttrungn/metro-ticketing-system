using BuildingBlocks.Domain.Events.Users;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Users.DTOs;
using UserService.Domain.Enums;

namespace UserService.Web.Consumers.StudentRequest;

public class UpdateStudentRequestDeclinedEventConsumer : IConsumer<UpdateStudentRequestDeclinedEvent>
{
    private readonly ILogger<UpdateStudentRequestDeclinedEventConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentRequestDeclinedEventConsumer(ILogger<UpdateStudentRequestDeclinedEventConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task Consume(ConsumeContext<UpdateStudentRequestDeclinedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("UpdateStationConsumer Message Received: {RouteId}", message.Id);

        var session = _unitOfWork.GetDocumentSession();
        var studentRequestReadModel = await session.LoadAsync<StudentRqReadModel>(message.Id);
        if (studentRequestReadModel != null)
        {
            studentRequestReadModel.StaffId = message.StaffId;
            studentRequestReadModel.Status = StudentRequestStatus.Declined;
            studentRequestReadModel.LastModifiedAt = message.LastModifiedAt;
           
            
            session.Update(studentRequestReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("UpdateStudentRequestConsumer Message Updated: {StudentRequestId}", message.Id);
            return;
        }
        _logger.LogWarning("UpdateStudentRequestConsumer Message Not Found: {StudentRequestId}", message.Id);
    }
}
