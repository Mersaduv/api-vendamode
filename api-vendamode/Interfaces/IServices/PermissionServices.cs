using api_vendamode.Entities.Users.Security;
using api_vendamode.Models;

namespace api_vendamode.Interfaces.IServices;

public interface IPermissionServices
{
    Task<ServiceResponse<Permission>> CreatePermissionAsync(string name, bool isActive, Guid? parentPermissionId);
    Task<ServiceResponse<List<Permission>>> GetAllPermissionsAsync();
    Task<ServiceResponse<Permission>> GetPermissionByIdAsync(Guid permissionId);
}