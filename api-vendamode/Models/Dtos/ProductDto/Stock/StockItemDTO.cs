using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api_vendace.Models.Dtos.ProductDto.Stock;

public class StockItemDTO
{
    public Guid Id { get; set; }
    public int StockId { get; set; }
    public string? Idx { get; set; }
    public IFormFile? ImageStock { get; set; }
    public List<Guid>? FeatureValueId { get; set; }
    public Guid? SizeId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public double? Discount { get; set; }
    [JsonExtensionData]
    [Column(TypeName = "jsonb")]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}