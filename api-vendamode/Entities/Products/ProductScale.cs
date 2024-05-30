using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Sizes;

namespace api_vendamode.Entities.Products;

public class ProductScale : BaseClass<Guid>
{
    public List<SizeIds>? Columns { get; set; }
    public List<SizeModel>? Rows { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
}