using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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

public class StudentRequestServiceService : IStudentRequestService
{   private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IAzureBlobService _azureBlobService;
    public StudentRequestServiceService(IUnitOfWork unitOfWork, IConfiguration configuration, IAzureBlobService azureBlobService)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _azureBlobService = azureBlobService;
    }
    
    public async Task<Guid> StudentRequestAsync(CreateStudentRequestCommand command, string userId)
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
        await studentRequestRepo.AddAsync(studentRequest);
        await _unitOfWork.SaveChangesAsync();
        return id;
    }

    public async Task<(IEnumerable<StudentRequestResponseDto>, int)> GetAsync(
        GetStudentRequestQuery query, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<StudentRequest, Guid>();
       Expression<Func<StudentRequest,bool>> filter = GetFilter(query);

       var studentRequests = await repo.GetPagedAsync(
           skip: query.CurrentPage * query.PageSize,
           take: query.PageSize,
           filters: new[] {filter},
           cancellationToken: cancellationToken);
           
        var totalPage = await repo.GetTotalPagesAsync(query.PageSize, new []{filter}, cancellationToken);

        var dtos = studentRequests.Select(sr => new StudentRequestResponseDto
        {
            Id = sr.Id,
            StaffId = sr.StaffId,
            FullName = sr.FullName,
            StudentCode = sr.StudentCode,
            StudentEmail = sr.StudentEmail,
            SchoolName = sr.SchoolName,
            DateOfBirth = sr.DateOfBirth,
            CustomerId = sr.CustomerId,
            StudentCardImageUrl = sr.StudentCardImageUrl,
            Status = sr.Status.ToString()
        });
        return (dtos, totalPage);

    
    }

    public Task<StudentRequestResponseDto?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<StudentRequest, Guid>();
        return repo.GetByIdAsync(requestId, cancellationToken)
            .ContinueWith(task =>
                {
                    var studentRequest = task.Result;
                    if(studentRequest == null) return null;
                    return new StudentRequestResponseDto
                    {
                        Id = studentRequest.Id,
                        StaffId = studentRequest.StaffId,
                        FullName = studentRequest.FullName,
                        StudentCode = studentRequest.StudentCode,
                        StudentEmail = studentRequest.StudentEmail,
                        SchoolName = studentRequest.SchoolName,
                        DateOfBirth = studentRequest.DateOfBirth, 
                        CustomerId = studentRequest.CustomerId,
                        StudentCardImageUrl = studentRequest.StudentCardImageUrl,
                        Status = studentRequest.Status.ToString()
                    };
                }, cancellationToken);
    }

    public async Task<Guid> UpdateAsync(UpdateStudentRequestCommand updateStudentRequestCommand, string? userId,CancellationToken cancellationToken = default)
    {
        var studentRequestRepo = _unitOfWork.GetRepository<StudentRequest, Guid>();
        var studentRequest = await studentRequestRepo.GetByIdAsync(updateStudentRequestCommand.Id);
        var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
        var staffId = staffRepo.Query().FirstOrDefault(s => s.ApplicationUserId == userId)?.Id;
        
        if (studentRequest == null)
            return Guid.Empty;

        studentRequest.StaffId = staffId; 
        studentRequest.Status = updateStudentRequestCommand.Status;
        
        await studentRequestRepo.UpdateAsync(studentRequest);
        await _unitOfWork.SaveChangesAsync();
        
        return studentRequest.Id;
    }
    #region Helper Methods
    private string GetFileType(string fileName)
    {
        return Path.GetExtension(fileName);
    }
    private Expression<Func<StudentRequest, bool>> GetFilter(GetStudentRequestQuery query)
    {
        return st =>
            (!query.Status.HasValue || st.Status == query.Status.Value) && (string.IsNullOrEmpty(query.SearchEmail) || st.StudentEmail.Contains(query.SearchEmail));
    }

    #endregion
}
