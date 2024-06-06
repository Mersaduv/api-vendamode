using System.Text.Json.Serialization;
using api_vendace.Models;

namespace api_vendace.Entities.Products;
public class FeatureValue : BaseClass<Guid>
{
    public required string Name { get; set; }
    public string? HexCode { get; set; }
    public int Count { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public Guid ProductFeatureId { get; set; }
    [JsonIgnore] 
    public virtual ProductFeature ProductFeature { get; set; } = default!;
}