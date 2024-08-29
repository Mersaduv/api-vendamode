using api_vendace.Models;

namespace api_vendamode.Models.Dtos.designDto;

public class HeaderTextUpsertDTO
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}