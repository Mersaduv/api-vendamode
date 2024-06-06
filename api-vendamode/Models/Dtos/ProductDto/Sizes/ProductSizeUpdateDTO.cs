using api_vendace.Entities.Products;
using api_vendace.Enums;
using api_vendace.Models;

namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class ProductSizeUpdateDTO : BaseClass<Guid>
{
    public SizeType SizeType { get; set; }
    public List<ProductSizeValues>? ProductSizeValues { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }
    public Guid CategoryId { get; set; }
}