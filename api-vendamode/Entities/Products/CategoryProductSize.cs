using api_vendace.Entities.Products;

namespace api_vendamode.Entities.Products;

public class CategoryProductSize
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    
    public Guid ProductSizeId { get; set; }
    public virtual ProductSize ProductSize { get; set; } = null!;
}