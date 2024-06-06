using api_vendace.Entities.Users.Security;
using api_vendace.Models;

namespace api_vendamode.Interfaces.IServices;

public interface IRoleServices
{
    Task<ServiceResponse<Role>> CreateRoleAsync(string title, bool isActive, List<Guid> permissionIds);
    Task<ServiceResponse<Role>> UpdateRoleAsync(Guid roleId, string title, bool isActive, List<Guid> permissionIds);
    Task<ServiceResponse<List<Role>>> GetAllRolesAsync();
    Task<ServiceResponse<Role>> GetRoleByIdAsync(Guid roleId);

}