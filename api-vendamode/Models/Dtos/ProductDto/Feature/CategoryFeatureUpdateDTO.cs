using api_vendace.Models.Dtos.ProductDto.Category;

namespace api_vendace.Models.Dtos.ProductDto.Feature;

public class CategoryFeatureUpdateDTO
{
    public Guid CategoryId { get; set; }
    public List<Guid>? FeatureIds { get; set; }
    public bool? HasSizeProperty { get; set; }
    public CategorySizeDTO? CategorySizes { get; set; }
}