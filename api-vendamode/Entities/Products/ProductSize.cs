using api_vendamode.Models;
using api_vendamode.Models.Dtos;

namespace api_vendamode.Entities.Products;

public class ProductSize : BaseClass<Guid>
{
    public SizeType SizeType { get; set; }
    public List<EntityImage<Guid, ProductSize>>? Images { get; set; }
    public virtual List<ProductSizeValues>? ProductSizeValues { get; set; }
    public virtual List<Sizes>? Sizes { get; set; }
    public bool IsDeleted { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }

}