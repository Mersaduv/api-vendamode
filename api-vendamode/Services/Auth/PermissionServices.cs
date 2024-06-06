using api_vendace.Data;
using api_vendace.Entities.Users.Security;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using Microsoft.EntityFrameworkCore;

namespace api_vendace.Services.Auth;

public class PermissionServices : IPermissionServices
{
    private readonly ApplicationDbContext _context;

    public PermissionServices(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<Permission>> CreatePermissionAsync(string name, bool isActive, Guid? parentPermissionId)
    {
        var response = new ServiceResponse<Permission>();
        try
        {
            var permission = new Permission
            {
                Name = name,
                IsActive = isActive,
                ParentPermissionId = parentPermissionId
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            response.Data = permission;
            response.Count = 1;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<List<Permission>>> GetAllPermissionsAsync()
    {
        var response = new ServiceResponse<List<Permission>>();
        try
        {
            var permissions = await _context.Permissions.Include(p => p.ChildPermissions).ToListAsync();
            response.Data = permissions;
            response.Count = permissions.Count;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<Permission>> GetPermissionByIdAsync(Guid permissionId)
    {
        var response = new ServiceResponse<Permission>();
        try
        {
            var permission = await _context.Permissions.Include(p => p.ChildPermissions).FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null) throw new Exception("Permission not found");

            response.Data = permission;
            response.Count = 1;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }
}
