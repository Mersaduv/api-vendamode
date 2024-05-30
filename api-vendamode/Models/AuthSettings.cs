namespace api_vendamode.Models;

public class AuthSettings
{
    public string TokenKey { get; set; } = string.Empty;
    public int TokenTimeout { get; set; }
    public int RefreshTokenTimeout { get; set; }
}
