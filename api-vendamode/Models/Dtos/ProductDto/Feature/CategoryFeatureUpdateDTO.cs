namespace api_vendamode.Models.Dtos.ProductDto.Feature;

public class CategoryFeatureUpdateDTO
{
    public Guid CategoryId { get; set; }
    public List<ProductFeatureUpdateDTO>? Features { get; set; }
}