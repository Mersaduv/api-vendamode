using api_vendamode.Entities.Users.Security;

namespace api_vendamode.Models.Dtos.AuthDto;

public class UserDTO : BaseClass<Guid>
{
    public EntityImageDto? ImageSrc { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string>? RoleNames { get; set; }
    public string? LastActivity { get; set; }
    public int OrderCount { get; set; }
    public string? City { get; set; }
    public bool Wallet { get; set; }
    public bool IsActive { get; set; }
    public UserSpecificationDTO UserSpecification { get; set; } = default!;
}