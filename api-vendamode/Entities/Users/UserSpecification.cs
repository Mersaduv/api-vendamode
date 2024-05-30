using api_vendamode.Entities.Products;
using api_vendamode.Enums;
using api_vendamode.Models;

namespace api_vendamode.Entities.Users;

public class UserSpecification : BaseClass<Guid>
{
    public Guid UserId { get; set; }
    public string? Role { get; set; }
    public bool IsActive { get; set; }
    public string? LastActivity { get; set; }
    public List<EntityImage<Guid, User>>? Images { get; set; }
    public List<EntityImage<Guid, UserSpecification>>? IdCardImages { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public byte[] Password { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
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