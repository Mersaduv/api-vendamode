using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using api_vendace.Entities;

namespace api_vendace.Models.Dtos.ProductDto.Stock;

public class StockItem
{
    public Guid Id { get; set; }
    public int StockId { get; set; }
    public Guid ProductId { get; set; }
    public List<EntityImage<Guid, StockItem>> Images { get; set; } = [];
    public List<Guid>? FeatureValueId { get; set; }
    public Guid? SizeId { get; set; }
    public string Idx { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double Price { get; set; }
    public double? Discount { get; set; }
    public double Weight { get; set; }
    public double PurchasePrice { get; set; }
    public int? OfferTime { get; set; }
    public bool IsHidden { get; set; }
    [JsonExtensionData]
    [Column(TypeName = "jsonb")]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? OfferStartTime { get; set; }
    public DateTime? OfferEndTime { get; set; }
}
