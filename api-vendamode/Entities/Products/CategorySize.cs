using api_vendace.Entities.Products;

namespace api_vendamode.Entities.Products;

public class CategorySize
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    
    public Guid SizeId { get; set; }
    public virtual Sizes Size { get; set; } = null!;
}