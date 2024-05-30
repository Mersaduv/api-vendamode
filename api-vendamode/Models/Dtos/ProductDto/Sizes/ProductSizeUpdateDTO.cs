using api_vendamode.Entities.Products;
using api_vendamode.Models;

namespace api_vendamode.Models.Dtos.ProductDto.Sizes;

public class ProductSizeUpdateDTO : BaseClass<Guid>
{
    public SizeType SizeType { get; set; }
    public List<ProductSizeValues>? ProductSizeValues { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }
    public Guid CategoryId { get; set; }
}