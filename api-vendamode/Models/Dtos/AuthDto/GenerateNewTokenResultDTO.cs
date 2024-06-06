namespace api_vendace.Models.Dtos.AuthDto;

public class GenerateNewTokenResultDTO
{
    public string MobileNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpireTime { get; set; }
    public bool LoggedIn { get; set; }
}