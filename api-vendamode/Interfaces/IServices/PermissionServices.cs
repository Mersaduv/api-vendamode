using api_vendace.Entities.Users.Security;
using api_vendace.Models;

namespace api_vendace.Interfaces.IServices;

public interface IPermissionServices
{
    Task<ServiceResponse<Permission>> CreatePermissionAsync(string name, bool isActive, Guid? parentPermissionId);
    Task<ServiceResponse<List<Permission>>> GetAllPermissionsAsync();
    Task<ServiceResponse<Permission>> GetPermissionByIdAsync(Guid permissionId);
}