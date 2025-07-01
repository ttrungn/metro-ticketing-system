using BuildingBlocks.Domain.Events.Users;
using MassTransit;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Users.DTOs;
using UserService.Domain.ValueObjects;

namespace UserService.Web.Consumers;

public class CreateCustomerConsumer : IConsumer<CreateCustomerEvent>
{
    private readonly ILogger<CreateCustomerConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateCustomerConsumer(ILogger<CreateCustomerConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<CreateCustomerEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("CreateCustomerEvent received");

        var session = _unitOfWork.GetDocumentSession();
        var customerReadModel = new CustomerReadModel()
        {
            Id = message.ApplicationUserId,
            FullName = new FullName(message.FirstName, message.LastName),
            Email = message.Email,
            CustomerId = message.CustomerId,
            IsStudent = message.IsStudent,
            CreatedAt = message.CreatedAt,
            LastModifiedAt = message.LastModifiedAt,
            DeletedAt = message.DeletedAt,
            DeleteFlag = message.DeleteFlag
        };
        session.Store(customerReadModel);
        
        await session.SaveChangesAsync();
    }
}
