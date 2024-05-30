using api_vendamode.Models;

namespace api_vendamode.Entities.Users.Security;

public class UserRefreshToken : BaseClass<Guid>
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    public string RefreshToken { get; set; } =string.Empty;
    public int RefreshTokenTimeout { get; set; }
    public bool IsValid { get; set; }
}