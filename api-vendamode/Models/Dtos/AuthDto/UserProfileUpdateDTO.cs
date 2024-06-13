namespace api_vendamode.Models.Dtos.AuthDto;

public class UserProfileUpdateDTO
{
    public string MobileNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string ShabaNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}