namespace api_vendamode.Models.Dtos.ProductDto.Stock;

public class StockItemDTO
{
    public Guid? FeatureId { get; set; }
    public Guid? SizeId { get; set; }
    public int Quantity { get; set; }
}