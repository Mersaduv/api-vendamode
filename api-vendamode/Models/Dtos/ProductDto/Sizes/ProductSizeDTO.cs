using api_vendace.Entities.Products;
using api_vendace.Enums;
using api_vendamode.Models.Dtos.ProductDto.Sizes;

namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class ProductSizeDTO : BaseClass<Guid>
{
    public SizeType SizeType { get; set; }
    public List<ProductSizeValuesDTO>? ProductSizeValues { get; set; }
    public EntityImageDto? ImagesSrc { get; set; }
}