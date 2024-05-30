using api_vendamode.Enums;
using api_vendamode.Models;
namespace api_vendamode.Entities.Products;
public class ProductFeature : BaseClass<Guid>
{
    public required string Name { get; set; }
    public virtual ICollection<FeatureValue>? Values { get; set; }
    public int Count { get; set; }
    public bool IsDeleted { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}