namespace api_vendamode.Models.Dtos.ProductDto.Sizes;

public class ProductSizeValuesDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ProductSizeId { get; set; }
}