using api_vendace.Models;

namespace api_vendace.Entities.Products;

public class Brand : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public List<EntityImage<Guid, Brand>>? Images { get; set; }
    public bool InSlider { get; set; }
    public bool IsActive { get; set; }
    public int Count { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDelete { get; set; }
    public virtual List<Product>? Products { get; set; }
}