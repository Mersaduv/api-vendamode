using api_vendace.Enums;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendamode.Entities.Products;

namespace api_vendace.Entities.Products;

public class ProductSize : BaseClass<Guid>
{
    public SizeType SizeType { get; set; }
    public List<EntityImage<Guid, ProductSize>>? Images { get; set; }
    public virtual List<ProductSizeProductSizeValue>? ProductSizeProductSizeValues { get; set; }
    public virtual List<Sizes>? Sizes { get; set; }
    public bool IsDeleted { get; set; }
    public virtual List<CategoryProductSize>? CategoryProductSizes { get; set; }
}