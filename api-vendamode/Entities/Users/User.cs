using api_vendamode.Entities.Users.Security;
using api_vendamode.Enums;
using api_vendamode.Models;

namespace api_vendamode.Entities.Users;

public class User : BaseClass<Guid>
{
    public UserTypes UserType { get; set; } = UserTypes.none;
    public virtual UserSpecification UserSpecification { get; set; } = default!;
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
