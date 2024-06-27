using api_vendace.Entities.Products;

namespace api_vendamode.Entities.Products;


public class ProductSizeProductSizeValue
{
    public Guid ProductSizeId { get; set; }
    public ProductSize? ProductSize { get; set; }
    public Guid ProductSizeValueId { get; set; }
    public ProductSizeValues? ProductSizeValue { get; set; }
}