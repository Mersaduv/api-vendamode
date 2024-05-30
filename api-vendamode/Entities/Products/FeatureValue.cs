using api_vendamode.Models;

namespace api_vendamode.Entities.Products;
public class FeatureValue : BaseClass<Guid>
{
    public required string Name { get; set; }
    public string? HexCode { get; set; }
    public int Count { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public Guid ProductFeatureId { get; set; }
    public virtual ProductFeature ProductFeature { get; set; } = default!;
}