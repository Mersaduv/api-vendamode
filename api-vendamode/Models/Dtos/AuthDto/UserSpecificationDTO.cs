using api_vendace.Entities.Users.Security;
using api_vendace.Enums;
using api_vendamode.Enums;

namespace api_vendace.Models.Dtos.AuthDto;

public class UserSpecificationDTO : BaseClass<Guid>
{
    public Guid UserId { get; set; }
    public UserTypes UserType { get; set; }
    public List<Role>? Roles { get; set; }
    public string Gender { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public EntityImageDto? ImageScr { get; set; }
    public EntityImageDto? IdCardImageSrc { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
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
    public string StoreName { get; set; } = string.Empty;
    public string StoreTelephone { get; set; } = string.Empty;
    public string StoreAddress { get; set; } = string.Empty;
    public string BussinessLicenseNumber { get; set; } = string.Empty;
    public bool IsActiveAddProduct { get; set; }
    public bool IsPublishProduct { get; set; }
    public bool IsSelectedAsSpecialSeller { get; set; }
    public CommissionType? CommissionType { get; set; } // nullable
    public string PercentageValue { get; set; } = string.Empty;
    public string SellerPerformance { get; set; } = string.Empty;
    public string TimelySupply { get; set; } = string.Empty;
    public string ShippingCommitment { get; set; } = string.Empty;
    public string NoReturns { get; set; } = string.Empty;
}