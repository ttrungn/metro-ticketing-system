using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interfaces.Repositories;
using UserService.Application.Common.Interfaces.Services;
using UserService.Application.Users.Commands.UpdateStaffById;
using UserService.Application.Users.DTOs;
using UserService.Application.Users.Queries.GetStaffs;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using UserService.Infrastructure.Services.Identity;

namespace UserService.Infrastructure.Services;

public class StaffService : IStaffService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public StaffService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    
    public async Task<StaffResponseDto?> GetStaffById(Guid id)
    {
        var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
        var staff = await staffRepo.Query()
            .Join(
                _userManager.Users, 
                c => c.ApplicationUserId, 
                u => u.Id, 
                (c, u) => new StaffResponseDto()
                {
                    Id = c.Id,
                    Email = u.Email!,
                    Name = $"{u.FullName.FirstName} {u.FullName.LastName}",
                    IsActive = !c.DeleteFlag
                })
            .FirstOrDefaultAsync(c => c.Id == id);
        
        return staff;
    }

    public async Task<IEnumerable<StaffResponseDto>> GetAllStaffs(GetStaffsQuery query)
    {
        var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
        var staffResponseDtos = await staffRepo.Query()
            .Join(
                _userManager.Users, 
                c => c.ApplicationUserId, 
                u => u.Id, 
                (c, u) => new StaffResponseDto()
                {
                    Id = c.Id,
                    Email = u.Email!,
                    Name = $"{u.FullName.FirstName} {u.FullName.LastName}",
                    IsActive = !c.DeleteFlag
                })
            .Where(c=> c.Email.Contains(query.Email.Trim()) && c.IsActive == query.IsActive)
            .Skip(query.Page * query.PageSize)
            .Take(query.PageSize)
            .OrderBy(c => c.Email)
            .ToListAsync();
        
        return staffResponseDtos;
    }
    
    public async Task<bool> UpdateStaffById(UpdateStaffByIdCommand request)
    {
        var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
        var staff = staffRepo.Query().FirstOrDefault(c => c.Id == request.Id);
        if (staff == null) return false;
        // update staff information
        await staffRepo.UpdateAsync(staff);
        
        // update user information
        var user = await _userManager.FindByIdAsync(staff.ApplicationUserId);
        if (user == null) return false;
        user.FullName = new FullName(request.FirstName, request.LastName);
        user.Email = request.Email;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            return false;
        }
        
        return await _unitOfWork.SaveChangesAsync() > 0 || identityResult.Succeeded;
    }
    
    public async Task DeleteStaffById(Guid id)
    {
        var staffRepo = _unitOfWork.GetRepository<Staff, Guid>();
        var staff = staffRepo.Query().FirstOrDefault(c => c.Id == id);
        if (staff == null) return;
        staff.DeleteFlag = true;
        await staffRepo.UpdateAsync(staff);
        await _unitOfWork.SaveChangesAsync();
    }
}
