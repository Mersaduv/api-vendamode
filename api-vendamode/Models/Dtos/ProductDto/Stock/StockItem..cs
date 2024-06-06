namespace api_vendace.Models.Dtos.ProductDto.Stock;

public class StockItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? FeatureId { get; set; }
    public Guid? SizeId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
