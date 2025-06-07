using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager;
    
    public StudentRequestServiceService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    
    public async Task<Guid> StudentRequestAsync(CreateStudentRequestCommand studentRequestCommand, string userId)
    {
        var studentRequestRepo = _unitOfWork.GetRepository<StudentRequest, Guid>();
        var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
        var customer = customerRepo.Query()
            .FirstOrDefault(c => c.ApplicationUserId == userId);

        if (customer == null)
            return Guid.Empty;
        
        var newStudentRequestId = Guid.NewGuid();
     
        var studentRequest = new StudentRequest
        {
            Id = newStudentRequestId,
            CustomerId = customer.Id,
            StudentCode = studentRequestCommand.StudentCode,
            StudentEmail = studentRequestCommand.StudentEmail,
            FullName = studentRequestCommand.FullName,
            DateOfBirth = studentRequestCommand.DateOfBirth,
            StudentCardImageUrl = studentRequestCommand.StudentCardImageUrl
        };
        await studentRequestRepo.AddAsync(studentRequest);
        await _unitOfWork.SaveChangesAsync();
        return newStudentRequestId;
    }

    public async Task<(IEnumerable<StudentRequestResponseDto>, int)> GetAsync(
        GetStudentRequestQuery query, int pagePerSize, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.GetRepository<StudentRequest, Guid>();
       Expression<Func<StudentRequest,bool>> filter = GetFilter(query);

       var studentRequests = await repo.GetPagedAsync(
           skip: query.Page * pagePerSize,
           take: pagePerSize,
           filters: new[] {filter},
           cancellationToken: cancellationToken);
           
        var totalPage = await repo.GetTotalPagesAsync(pagePerSize, new []{filter}, cancellationToken);

        var dtos = studentRequests.Select(sr => new StudentRequestResponseDto
        {
            Id = sr.Id,
            StaffId = sr.StaffId,
            FullName = sr.FullName,
            StudentCode = sr.StudentCode,
            StudentEmail = sr.StudentEmail,
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

    private Expression<Func<StudentRequest, bool>> GetFilter(GetStudentRequestQuery query)
    {
        return st =>
            (!query.Status.HasValue || st.Status == query.Status.Value) && (string.IsNullOrEmpty(query.SearchEmail) || st.StudentEmail.Contains(query.SearchEmail));
    }

    #endregion
}
