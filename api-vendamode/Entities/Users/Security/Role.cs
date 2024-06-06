using api_vendace.Models;

namespace api_vendace.Entities.Users.Security;

public class Role : BaseClass<Guid>
{
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
