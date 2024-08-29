using api_vendace.Models;

namespace api_vendamode.Models.Dtos.designDto;

public class HeaderTextDTO : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}