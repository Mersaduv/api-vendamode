using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Sizes;

namespace api_vendace.Entities.Products;

public class ProductScale : BaseClass<Guid>
{
    public List<SizeIds>? Columns { get; set; }
    public List<SizeModel>? Rows { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
}