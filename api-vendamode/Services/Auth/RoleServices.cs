using api_vendamode.Data;
using api_vendamode.Entities.Users.Security;
using api_vendamode.Models;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Auth;

public class RoleServices
{
    private readonly ApplicationDbContext _context;

    public RoleServices(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<Role>> CreateRoleAsync(string title, bool isActive, List<Guid> permissionIds)
    {
        var response = new ServiceResponse<Role>();
        try
        {
            var role = new Role
            {
                Title = title,
                IsActive = isActive,
                Permissions = await _context.Permissions.Where(p => permissionIds.Contains(p.Id)).ToListAsync()
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            response.Data = role;
            response.Count = 1;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<Role>> UpdateRoleAsync(Guid roleId, string title, bool isActive, List<Guid> permissionIds)
    {
        var response = new ServiceResponse<Role>();
        try
        {
            var role = await _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null) throw new Exception("سمت مورد نظر پیدا نشد");

            role.Title = title;
            role.IsActive = isActive;
            role.Permissions = await _context.Permissions.Where(p => permissionIds.Contains(p.Id)).ToListAsync();

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            response.Data = role;
            response.Count = 1;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<List<Role>>> GetAllRolesAsync()
    {
        var response = new ServiceResponse<List<Role>>();
        try
        {
            var roles = await _context.Roles.Include(r => r.Permissions).ToListAsync();
            response.Data = roles;
            response.Count = roles.Count;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<Role>> GetRoleByIdAsync(Guid roleId)
    {
        var response = new ServiceResponse<Role>();
        try
        {
            var role = await _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null) throw new Exception("سمت مورد نظر پیدا نشد");

            response.Data = role;
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
