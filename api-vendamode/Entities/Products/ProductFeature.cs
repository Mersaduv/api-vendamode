using System.Text.Json.Serialization;
using api_vendace.Enums;
using api_vendace.Models;
using api_vendamode.Entities.Products;
namespace api_vendace.Entities.Products;
public class ProductFeature : BaseClass<Guid>
{
    public required string Name { get; set; }
    public virtual List<FeatureValue>? Values { get; set; }
    public int Count { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? ProductId { get; set; }
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public Guid? CategoryId { get; set; }
    [JsonIgnore]
    public virtual List<CategoryProductFeature>? CategoryProductFeatures { get; set; }
}
