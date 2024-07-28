using api_vendace.Entities.Products;

namespace api_vendamode.Entities.Products;

public class CategoryProductFeature
{
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; } = default!;
    public Guid ProductFeatureId { get; set; }
    public virtual ProductFeature ProductFeature { get; set; } = default!;
}
