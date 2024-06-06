namespace api_vendace.Models.Dtos.AuthDto;

public class GenerateNewTokenDTO
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}