using api_vendamode.Models;

namespace api_vendamode.Entities.Products;

public class ProductColor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HashCode { get; set; } = string.Empty;
}