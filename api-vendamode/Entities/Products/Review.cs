using api_vendace.Entities.Users;
using api_vendace.Models;

namespace api_vendace.Entities.Products;
public class Review : BaseClass<Guid>
{
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int Status { get; set; }
    public List<Points>? PositivePoints { get; set; }
    public List<Points>? NegativePoints { get; set; }
    public List<EntityImage<Guid, Review>> Images { get; set; } = [];
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = default!;
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }
}