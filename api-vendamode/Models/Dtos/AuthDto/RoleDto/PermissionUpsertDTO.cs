namespace api_vendamode.Models.Dtos.AuthDto.RoleDto;

public class PermissionUpsertDTO
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? ParentPermissionId { get; set; }
}