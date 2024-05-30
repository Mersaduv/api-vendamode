using api_vendamode.Entities.Products;

namespace api_vendamode.Models.Dtos.ProductDto.Sizes;

public class ProductSizeDTO : BaseClass<Guid>
{
    public SizeType SizeType { get; set; }
    public List<ProductSizeValues>? ProductSizeValues { get; set; }
    public List<Entities.Products.Sizes>? Sizes { get; set; }
    public EntityImageDto? ImagesSrc { get; set; }
}