namespace api_vendace.Models.Dtos.ProductDto.Feature;

public class CategoryFeatureUpdateDTO
{
    public Guid CategoryId { get; set; }
    public List<Guid>? FeatureIds { get; set; }
    public List<Guid>? SizeList { get; set; }
}