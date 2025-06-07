using System.Text.Json.Serialization;
using BuildingBlocks.Domain.Constants;
using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Models;
using UserService.Application.Common.Security;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Users.Commands.StudentRequest;

// [Authorize(Roles = Roles.Customer)]
public record CreateStudentRequestCommand : IRequest<ServiceResponse<Guid>>
{
    public string StudentCode { get; init; } = null!;
    public string StudentEmail { get; init; } = null!;
    public FullName FullName { get; init; } = null!;
    public DateTimeOffset DateOfBirth { get; init; } 
    [JsonIgnore]
    public string StudentCardImageUrl { get; set; } = null!;
}
public class StudentRequestResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = [];
}

public class StudentRequestCommandValidator : AbstractValidator<CreateStudentRequestCommand>
{
    public StudentRequestCommandValidator()
    {
        RuleFor(x => x.StudentCode)
            .NotEmpty().WithMessage("Mã sinh viên là bắt buộc.");
        
        RuleFor(x => x.StudentEmail)
            .NotEmpty().WithMessage("Vui lòng nhập email sinh viên.")
            .EmailAddress().WithMessage("Email không hợp lệ.");
        
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên là bắt buộc.");
        
        RuleFor(x => x.DateOfBirth)
            .NotNull().WithMessage("Ngày sinh là bắt buộc.")
            .LessThan(DateTimeOffset.Now).WithMessage("Ngày sinh phải trước ngày hiện tại.");
    }
}
public class StudentRequestCommandHandler : IRequestHandler<CreateStudentRequestCommand, ServiceResponse<Guid>>
{
    private readonly ILogger<StudentRequestCommandHandler> _logger;
    private readonly IStudentRequestService _studentRequestService;
    private readonly IUser _currentUser;
    public StudentRequestCommandHandler(ILogger<StudentRequestCommandHandler> logger, 
                                        IStudentRequestService studentRequestService,
                                        IUser currentUser)
    {
        _logger = logger;
        _studentRequestService = studentRequestService;
        _currentUser = currentUser;
    }

    public async Task<ServiceResponse<Guid>> Handle(CreateStudentRequestCommand request, CancellationToken cancellationToken)
    {
         var userId = _currentUser.Id;
        if (string.IsNullOrEmpty(request.StudentCardImageUrl))
        {
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Ảnh thẻ sinh viên là bắt buộc.",
                Data = Guid.Empty
            };
        }
        _logger.LogInformation("Login attempt started for Email: {StudentEmail}", request.StudentEmail);
        var studentRequestId = await _studentRequestService.StudentRequestAsync(request, userId!);
        return new ServiceResponse<Guid>
        {
            Succeeded = true,
            Message = "Created student request successfully.",
            Data = studentRequestId
        };
    }
}
