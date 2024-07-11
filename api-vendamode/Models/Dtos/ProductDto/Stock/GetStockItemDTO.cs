using api_vendace.Entities;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace api_vendamode.Models.Dtos.ProductDto.Stock;

public class GetStockItemDTO : BaseClass<Guid>
{
    public int StockId { get; set; }
    public Guid ProductId { get; set; }
    public List<EntityImageDto> ImagesSrc { get; set; } = [];
    public List<Guid>? FeatureValueId { get; set; }
    public Guid? SizeId { get; set; }
    public string Idx { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double Price { get; set; }
    public double? Discount { get; set; }
    [JsonExtensionData]
    [Column(TypeName = "jsonb")]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}