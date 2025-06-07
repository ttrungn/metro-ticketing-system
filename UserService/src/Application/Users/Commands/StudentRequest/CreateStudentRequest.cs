using System.Text.Json.Serialization;
using BuildingBlocks.Response;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Models;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Users.Commands.StudentRequest;

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
            .NotEmpty().WithMessage("Student code is required.");
        
        RuleFor(x => x.StudentEmail)
            .NotEmpty().WithMessage("Student email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("First name is required.");
        
        RuleFor(x => x.DateOfBirth)
            .NotNull().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.");
        // RuleFor(x => x.StudentCardImageUrl)
        //     .NotEmpty().WithMessage("Student card image URL is required.")
        //     .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
        //     .WithMessage("Invalid URL format for student card image.");
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
        if (string.IsNullOrEmpty(userId))
        {
            return new ServiceResponse<Guid>()
            {
                Succeeded = false,
                Message = "Please login to continue.",
                Data = Guid.Empty
            };
        }
        _logger.LogInformation("Login attempt started for Email: {StudentEmail}", request.StudentEmail);
        var studentRequestId = await _studentRequestService.StudentRequestAsync(request, userId);
        return new ServiceResponse<Guid>
        {
            Succeeded = true,
            Message = "Created student request successfully.",
            Data = studentRequestId
        };
    }
}
