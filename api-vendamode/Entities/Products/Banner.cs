using api_vendace.Entities;
using api_vendace.Models;

namespace api_vendamode.Entities.Products;

public class Banner : BaseClass<Guid>
{
    public Guid? CategoryId { get; set; }
    public EntityImage<Guid, Banner>? Image { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Index { get; set; }
}