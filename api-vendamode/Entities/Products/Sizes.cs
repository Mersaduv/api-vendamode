using api_vendamode.Models;

namespace api_vendamode.Entities.Products;

public class Sizes : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public Guid ProductSizeId { get; set; }
    public ProductSize? ProductSize { get; set; }
}