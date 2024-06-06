using api_vendace.Models;

namespace api_vendace.Entities.Users.Security;

public class UserRefreshToken : BaseClass<Guid>
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    public string RefreshToken { get; set; } =string.Empty;
    public int RefreshTokenTimeout { get; set; }
    public bool IsValid { get; set; }
}