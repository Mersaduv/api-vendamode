using api_vendamode.Entities.Products;

namespace api_vendamode.Models.Dtos.ProductDto.Sizes;

public class ProductSizeCreateDTO
{
    public SizeType SizeType { get; set; }
    public List<ProductSizeValues>? ProductSizeValues { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ProductId { get; set; }
}