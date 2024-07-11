using api_vendace.Entities.Products;
using api_vendace.Models.Dtos.ProductDto.Sizes;

namespace api_vendamode.Models.Dtos.ProductDto;

public class GetCategoryFeaturesByCategory
{
    public List<ProductFeature>? ProductFeatures { get; set; }
    public List<ProductSizeDTO>? ProductSizes { get; set; }
    public List<SizeDTO>? SizeDTOs { get; set; }
}