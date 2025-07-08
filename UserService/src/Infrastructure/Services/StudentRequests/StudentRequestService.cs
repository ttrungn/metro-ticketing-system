using System.Linq.Expressions;
using BuildingBlocks.Domain.Events.Users;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Common.Models;
using UserService.Application.Users.Commands.StudentRequest;
using UserService.Application.Users.DTOs;
using UserService.Application.Users.Queries;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Infrastructure.Services.Identity;

namespace UserService.Infrastructure.Services.StudentRequests;

public class StudentRequestService : IStudentRequestService
{   private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<StudentRequestService> _logger;
    public StudentRequestService(IUnitOfWork unitOfWork, IConfiguration configuration, IAzureBlobService azureBlobService, ILogger<StudentRequestService> logger)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _azureBlobService = azureBlobService;
        _logger = logger;
    }
    
    public async Task<Guid> CreateStudentRequestAsync(CreateStudentRequestCommand command, string userId)
    {
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = customerRepo.Query()
            .FirstOrDefault(c => c.ApplicationUserId == userId);

        if (customer == null)
            return Guid.Empty;
        
        var id = Guid.NewGuid();
        var studentRequestRepo = _unitOfWork.GetRepository<StudentRequest, Guid>();
        var studentCardImageUrl = "empty";
        
        if (command.StudentCardImageStream != null && command.StudentCardImageName != null)
        {
            var blobName = id + GetFileType(command.StudentCardImageName);
            var containerName = _configuration["Azure:BlobStorageSettings:StudentVerificationImagesContainerName"] ?? "student-verification-images";
            var blobUrl = await _azureBlobService.UploadAsync(
                command.StudentCardImageStream,
                blobName,
                containerName);
            studentCardImageUrl = blobUrl;
        }
        var studentRequest = new StudentRequest
        {
            Id = id,
            CustomerId = customer.Id,
            StudentCode = command.StudentCode,
            StudentEmail = command.StudentEmail,
            SchoolName = command.SchoolName,
            FullName = command.FullName,
            DateOfBirth = command.DateOfBirth,
            StudentCardImageUrl = studentCardImageUrl
        };
        studentRequest.AddDomainEvent(new CreateStudentRequestEvent()
        {
            Id = studentRequest.Id,
            CustomerId = studentRequest.CustomerId,
            StaffId = null, 
            StudentCode = studentRequest.StudentCode,
            StudentEmail = studentRequest.StudentEmail,
            SchoolName = studentRequest.SchoolName,
            FirstName = studentRequest.FullName.FirstName,
            LastName = studentRequest.FullName.LastName,
            DateOfBirth = studentRequest.DateOfBirth,
            StudentCardImageUrl = studentRequest.StudentCardImageUrl
        });
        await studentRequestRepo.AddAsync(studentRequest);
        await _unitOfWork.SaveChangesAsync();
        return id;
    }

    public async Task<(IEnumerable<StudentRqReadModel>, int)> GetAsync(
        GetStudentRequestQuery query, CancellationToken cancellationToken = default)
    {
        var session = _unitOfWork.GetDocumentSession();
       Expression<Func<StudentRqReadModel,bool>> filter = GetFilter(query);

       var studentRequests = await QueryableExtensions.ToListAsync(session.Query<StudentRqReadModel>()
            .Where(filter)
            .Skip(query.CurrentPage * query.PageSize)
            .Take(query.PageSize)
            .AsNoTracking()
           , cancellationToken);
           
        var totalCount = await QueryableExtensions.ToListAsync(session.Query<StudentRqReadModel>()
            .Where(filter)
            .AsNoTracking()
            , cancellationToken);
        var totalPage = (int)Math.Ceiling((double)totalCount.Count / query.PageSize);
        return (studentRequests, totalPage);

    
    }

    public async Task<StudentRqReadModel?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var session = _unitOfWork.GetDocumentSession();
        var studentRequest = await QueryableExtensions.FirstOrDefaultAsync(session.Query<StudentRqReadModel>()
            .Where(s => s.Id == requestId)
            .AsNoTracking(), cancellationToken);
        return studentRequest;
    }

    public async Task<Guid> UpdateAsync(UpdateStudentRequestCommand updateStudentRequestCommand, string? userId,CancellationToken cancellationToken = default)
    {
        var studentRequestRepo = _unitOfWork.GetRepository<StudentRequest, Guid>();
        var studentRequest = await studentRequestRepo.GetByIdAsync(updateStudentRequestCommand.Id);
        var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
        var staffId = staffRepo.Query().FirstOrDefault(s => s.ApplicationUserId == userId)?.Id;
        if (staffId == null)
        {
            _logger.LogWarning("Staff with ApplicationUserId {UserId} not found.", userId);
            return Guid.Empty;
        }

        if (studentRequest == null)
            return Guid.Empty;

        studentRequest.StaffId = staffId; 
        studentRequest.Status = updateStudentRequestCommand.Status;
        if (updateStudentRequestCommand.Status == StudentRequestStatus.Approved)
        {
            studentRequest.AddDomainEvent(new UpdateStudentRequestApproveEvent
            {
                Id = studentRequest.Id, 
                StaffId = studentRequest.StaffId ?? Guid.Empty,
            });
        }
        if (updateStudentRequestCommand.Status == StudentRequestStatus.Declined)
        {
            studentRequest.AddDomainEvent(new UpdateStudentRequestDeclinedEvent
            {
                Id = studentRequest.Id, 
                StaffId = studentRequest.StaffId ?? Guid.Empty,
            });
        }
        await studentRequestRepo.UpdateAsync(studentRequest);
        await _unitOfWork.SaveChangesAsync();
        
        return studentRequest.Id;
    }
    
    #region Helper Methods
    private string GetFileType(string fileName)
    {
        return Path.GetExtension(fileName);
    }
    private Expression<Func<StudentRqReadModel, bool>> GetFilter(GetStudentRequestQuery query)
    {
        if (!query.Status.HasValue && string.IsNullOrEmpty(query.SearchEmail))
        {
            return st => true;
        }
        if (query.Status.HasValue && string.IsNullOrEmpty(query.SearchEmail))
        {
            return st => st.Status == query.Status;
        }

        return st =>
            (!query.Status.HasValue || st.Status == query.Status) &&
            (string.IsNullOrEmpty(query.SearchEmail) ||
             (st.StudentEmail != null && st.StudentEmail.Contains(query.SearchEmail)));
    }



    #endregion
}
