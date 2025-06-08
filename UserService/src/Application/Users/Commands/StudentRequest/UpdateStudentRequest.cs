using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Security;
using UserService.Application.Users.Queries;
using UserService.Domain.Enums;

namespace UserService.Application.Users.Commands.StudentRequest;
// [Authorize(Roles = Roles.Staff)]
public record UpdateStudentRequestCommand : IRequest<ServiceResponse<Guid>>
{
    public Guid Id { get; init; }
    public StudentRequestStatus Status { get; init; }
}

public class UpdateStudentRequestCommandValidator : AbstractValidator<UpdateStudentRequestCommand>
{
    public UpdateStudentRequestCommandValidator()
    {
        RuleFor(x => x.Status).NotEmpty().WithMessage("Xin vui lòng chọn trạng thái");
    }
}

public class UpdateStudentRequestCommandHandler : IRequestHandler<UpdateStudentRequestCommand, ServiceResponse<Guid>>
{
    private readonly IStudentRequestService _requestService;
    private readonly ILogger<UpdateStudentRequestCommand> _logger;
    private readonly IUser _user;

    public UpdateStudentRequestCommandHandler(IStudentRequestService requestService, 
        ILogger<UpdateStudentRequestCommand> logger, IUser user)
    {
        _requestService = requestService;
        _logger = logger;
        _user = user;
    }

    public async Task<ServiceResponse<Guid>> Handle(UpdateStudentRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;
        var id = await _requestService.UpdateAsync(request, userId, cancellationToken);
        if (id == Guid.Empty)
        {
            _logger.LogError("Failed to update student request with ID {Id}", request.Id);
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message ="Failed to update student request." ,
                Data = Guid.Empty
            };
        }
        _logger.LogInformation("Successfully updated student request with ID {Id}", request.Id);
        return new ServiceResponse<Guid>()
        {
            Succeeded = true,
            Message = "Student request updated successfully.",
            Data = id
        };
    }
    
}
