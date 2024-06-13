using api_vendace.Entities.Users.Security;
using api_vendace.Enums;
using api_vendace.Models;

namespace api_vendace.Entities.Users;

public class User : BaseClass<Guid>
{
    public UserTypes UserType { get; set; } = UserTypes.none;

    public List<EntityImage<Guid, User>>? Images { get; set; }
    public List<Address> Addresses { get; set; } = new List<Address>();
    public virtual UserSpecification UserSpecification { get; set; } = default!;
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
