using api_vendace.Models;

namespace api_vendace.Entities.Products;

public class Sizes : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ProductSizeId { get; set; }
    public virtual ProductSize? ProductSize { get; set; }
}