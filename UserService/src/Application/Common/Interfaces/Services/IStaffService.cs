using UserService.Application.Users.Commands.UpdateStaffById;
using UserService.Application.Users.DTOs;
using UserService.Application.Users.Queries.GetStaffs;

namespace UserService.Application.Common.Interfaces.Services;

public interface IStaffService
{
    Task<StaffResponseDto?> GetStaffById(Guid id);
    Task<IEnumerable<StaffResponseDto>> GetAllStaffs(GetStaffsQuery query);
    Task<bool> UpdateStaffById(UpdateStaffByIdCommand request);
    Task DeleteStaffById(Guid id);
    Task ActivateStaffById(Guid id);
}
