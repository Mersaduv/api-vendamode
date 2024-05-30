using api_vendamode.Entities.Users;
using api_vendamode.Models;

namespace api_vendamode.Entities.Products;
public class Review : BaseClass<Guid>
{
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int Status { get; set; }
    public List<Points>? PositivePoints { get; set; }
    public List<Points>? NegativePoints { get; set; }
    public List<EntityImage<Guid, Review>> Images { get; set; } = [];
    public List<EntityImage<Guid, Product>> ProductImage { get; set; } = [];
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = default!;
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }
}