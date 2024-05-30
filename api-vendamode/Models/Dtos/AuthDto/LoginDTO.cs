namespace api_vendamode.Models.Dtos.AuthDto;

public class LoginDTO
{
    public string MobileNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int RefreshTokenExpireTime { get; set; }
    public int ExpireTime { get; set; }
}