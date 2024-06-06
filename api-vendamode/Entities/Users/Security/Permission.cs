using api_vendace.Models;

namespace api_vendace.Entities.Users.Security;

public class Permission : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? ParentPermissionId { get; set; }
    public virtual Permission? ParentPermission { get; set; }
    public virtual ICollection<Permission> ChildPermissions { get; set; } = new List<Permission>();
}
