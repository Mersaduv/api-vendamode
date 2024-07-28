using api_vendace.Entities.Products;
using api_vendace.Models;

namespace api_vendamode.Models.Dtos;

public class ProductFeatureDto:BaseClass<Guid>
{
    public required string Name { get; set; }
    public virtual List<FeatureValue>? Values { get; set; }
    public int Count { get; set; }
    public int ValueCount { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? CategoryId { get; set; }
}