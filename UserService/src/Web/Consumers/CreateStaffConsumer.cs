using BuildingBlocks.Domain.Events.Users;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Users.DTOs;
using UserService.Domain.ValueObjects;

namespace UserService.Web.Consumers;

public class CreateStaffConsumer : IConsumer<CreateStaffEvent>
{
    private readonly ILogger<CreateStaffConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStaffConsumer(ILogger<CreateStaffConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateStaffEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateCustomerEvent received");

        var session = _unitOfWork.GetDocumentSession();
        var staffReadModel = new StaffReadModel()
        {
            Id = message.ApplicationUserId,
            FullName = new FullName(message.FirstName, message.LastName),
            Email = message.Email,
            StaffId = message.StaffId,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag
        };
        session.Store(staffReadModel);
        
        await session.SaveChangesAsync();
    }
}
