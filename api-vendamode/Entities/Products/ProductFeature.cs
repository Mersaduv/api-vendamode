using System.Text.Json.Serialization;
using api_vendace.Enums;
using api_vendace.Models;
namespace api_vendace.Entities.Products;
public class ProductFeature : BaseClass<Guid>
{
    public required string Name { get; set; }
    public virtual List<FeatureValue>? Values { get; set; }
    public int Count { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? ProductId { get; set; }
    [JsonIgnore]
    public virtual Product? Product { get; set; }
    public Guid? CategoryId { get; set; }
    [JsonIgnore]
    public virtual Category? Category { get; set; }
}
