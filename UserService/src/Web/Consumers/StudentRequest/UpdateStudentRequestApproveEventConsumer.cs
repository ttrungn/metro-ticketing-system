using BuildingBlocks.Domain.Events.Users;
using Marten;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Users.DTOs;
using UserService.Domain.Enums;

namespace UserService.Web.Consumers.StudentRequest;

public class UpdateStudentRequestApproveEventConsumer : IConsumer<UpdateStudentRequestApproveEvent>
{
    private readonly ILogger<UpdateStudentRequestApproveEventConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentRequestApproveEventConsumer(ILogger<UpdateStudentRequestApproveEventConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task Consume(ConsumeContext<UpdateStudentRequestApproveEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("UpdateStationConsumer Message Received: {StudentRqId}", message.Id);
   
        var session = _unitOfWork.GetDocumentSession();
        var customerSession = _unitOfWork.GetDocumentSession();
        var studentRequestReadModel = await session.LoadAsync<StudentRqReadModel>(message.Id);
        var customerReadModel = await customerSession.Query<CustomerReadModel>()
            .FirstOrDefaultAsync(c => c.CustomerId == message.CustomerId);
        if (studentRequestReadModel != null && customerReadModel != null)
        {
            studentRequestReadModel.StaffId = message.StaffId;
            studentRequestReadModel.Status = StudentRequestStatus.Approved;
            studentRequestReadModel.LastModifiedAt = message.LastModifiedAt;
            customerReadModel.IsStudent = true;
            customerSession.Update(customerReadModel);
            await customerSession.SaveChangesAsync();
            session.Update(studentRequestReadModel);
            await session.SaveChangesAsync();
            _logger.LogInformation("UpdateStudentRequestConsumer Message Updated: {StudentRequestId}", message.Id);
            return;
        }
        _logger.LogWarning("UpdateStudentRequestConsumer Message Not Found: {StudentRequestId}", message.Id);
    }
}
