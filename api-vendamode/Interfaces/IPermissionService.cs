namespace api_vendamode.Interfaces;

public interface IPermissionService
{
    Task<bool> CheckPermission(Guid userId, string permissionFlag);

}