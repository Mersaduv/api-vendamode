using System.Reflection;
using api_vendace.Enums;
using api_vendamode.Enums;

namespace api_vendace.Models.Dtos.AuthDto;

public class UserUpsertDTO
{
    public Guid? Id { get; set; }
    public UserTypes UserType { get; set; }
    public List<Guid>? RoleIds { get; set; }
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
    // Supplier
    // store specification 
    public string StoreName { get; set; } = string.Empty;
    public string StoreTelephone { get; set; } = string.Empty;
    public string StoreAddress { get; set; } = string.Empty;
    public string BussinessLicenseNumber { get; set; } = string.Empty;
    // store setting
    public bool IsActiveAddProduct { get; set; }
    public bool IsPublishProduct { get; set; }
    public bool IsSelectedAsSpecialSeller { get; set; }
    public CommissionType? CommissionType { get; set; }
    public string PercentageValue { get; set; } = string.Empty;
    public string SellerPerformance { get; set; } = string.Empty;
    public string TimelySupply { get; set; } = string.Empty;
    public string ShippingCommitment { get; set; } = string.Empty;
    public string NoReturns { get; set; } = string.Empty;
    public static async ValueTask<UserUpsertDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : new List<IFormFile>();
        if (!Guid.TryParse(form["Id"], out var id))
        {
        }
        var idCardThumbnailFiles = form.Files.GetFiles("IdCardThumbnail");
        var idCardThumbnail = idCardThumbnailFiles.Any() ? idCardThumbnailFiles.ToList() : new List<IFormFile>();

        var userType = Enum.Parse<UserTypes>(form["UserType"]!);
        var isActive = bool.Parse(form["IsActive"].ToString()!);
        var roleIds = form["RoleIds"]
            .SelectMany(id => id!.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(Guid.Parse)
            .ToList();

        var mobileNumber = form["MobileNumber"].ToString();
        var passCode = form["PassCode"].ToString();
        var firstName = form["FirstName"].ToString();
        var familyName = form["FamilyName"].ToString();
        var fatherName = form["FatherName"].ToString();
        var telePhone = form["TelePhone"].ToString();
        var province = form["Province"].ToString();
        var city = form["City"].ToString();
        var postalCode = form["PostalCode"].ToString();
        var firstAddress = form["FirstAddress"].ToString();
        var secondAddress = form["SecondAddress"].ToString();
        var birthDate = form["BirthDate"].ToString();
        var idNumber = form["IdNumber"].ToString();
        var nationalCode = form["NationalCode"].ToString();
        var bankAccountNumber = form["BankAccountNumber"].ToString();
        var shabaNumber = form["ShabaNumber"].ToString();
        var note = form["Note"].ToString();

        // Supplier fields
        var storeName = form["StoreName"].ToString();
        var storeTelephone = form["StoreTelephone"].ToString();
        var storeAddress = form["StoreAddress"].ToString();
        var bussinessLicenseNumber = form["BussinessLicenseNumber"].ToString();

        var isActiveAddProductForm = bool.TryParse(form["isActiveAddProduct"], out var isActiveAddProduct) && isActiveAddProduct;
        var isPublishProductForm = bool.TryParse(form["isPublishProduct"], out var isPublishProduct) && isPublishProduct;
        var isSelectedAsSpecialSellerForm = bool.TryParse(form["isSelectedAsSpecialSeller"], out var isSelectedAsSpecialSeller) && isSelectedAsSpecialSeller;

        CommissionType? commissionType = null;
        if (Enum.TryParse<CommissionType>(form["CommissionType"], out var parsedCommissionType))
        {
            commissionType = parsedCommissionType;
        }
        var percentageValue = form["PercentageValue"].ToString();
        var sellerPerformance = form["SellerPerformance"].ToString();
        var timelySupply = form["TimelySupply"].ToString();
        var shippingCommitment = form["ShippingCommitment"].ToString();
        var noReturns = form["NoReturns"].ToString();

        return new UserUpsertDTO
        {
            Id = id,
            UserType = userType,
            IsActive = isActive,
            RoleIds = roleIds,
            Thumbnail = thumbnail,
            IdCardThumbnail = idCardThumbnail,
            MobileNumber = mobileNumber!,
            PassCode = passCode!,
            FirstName = firstName!,
            FamilyName = familyName!,
            FatherName = fatherName!,
            TelePhone = telePhone!,
            Province = province!,
            City = city!,
            PostalCode = postalCode!,
            FirstAddress = firstAddress!,
            SecondAddress = secondAddress!,
            BirthDate = birthDate!,
            IdNumber = idNumber!,
            NationalCode = nationalCode!,
            BankAccountNumber = bankAccountNumber!,
            ShabaNumber = shabaNumber!,
            Note = note!,
            // Supplier fields
            StoreName = storeName!,
            StoreTelephone = storeTelephone!,
            StoreAddress = storeAddress!,
            BussinessLicenseNumber = bussinessLicenseNumber!,
            IsActiveAddProduct = isActiveAddProductForm,
            IsPublishProduct = isPublishProductForm,
            IsSelectedAsSpecialSeller = isSelectedAsSpecialSellerForm,
            CommissionType = commissionType,
            PercentageValue = percentageValue!,
            SellerPerformance = sellerPerformance!,
            TimelySupply = timelySupply!,
            ShippingCommitment = shippingCommitment!,
            NoReturns = noReturns!
        };
    }

}
