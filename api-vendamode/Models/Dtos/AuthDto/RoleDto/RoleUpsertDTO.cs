namespace api_vendamode.Models.Dtos.AuthDto.RoleDto;

public class RoleUpsertDTO
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}