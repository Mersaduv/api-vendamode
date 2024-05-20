using api_vendamode.Models;

namespace api_vendamode.Entities.Products;

public class Brand : BaseClass<int>
{
    public string Name { get; set; } = string.Empty;
    public virtual List<Product>? Products { get; set; }
}