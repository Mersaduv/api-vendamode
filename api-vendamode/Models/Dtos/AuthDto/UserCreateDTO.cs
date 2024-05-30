using api_vendamode.Enums;

namespace api_vendamode.Models.Dtos.AuthDto;

public class UserCreateDTO
{
    public UserTypes UserType { get; set; }
    public Guid? RoleId { get; set; }
    public bool IsActive { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }
    public List<IFormFile>? IdCardThumbnail { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string PassCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string TelePhone { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string FirstAddress { get; set; } = string.Empty;
    public string SecondAddress { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string ShabaNumber { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}