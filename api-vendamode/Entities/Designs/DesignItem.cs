
using api_vendace.Entities;
using api_vendace.Models;

namespace api_vendamode.Entities.Designs;
public class DesignItem : BaseClass<Guid>
{
    public string Title { get; set; } = string.Empty;
    public EntityImage<Guid, DesignItem>? Image { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Index { get; set; }
}