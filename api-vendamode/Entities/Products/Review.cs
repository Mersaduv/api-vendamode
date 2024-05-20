using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Review;
namespace api_vendamode.Entities.Products;

public class Review : BaseClass<Guid>
{
    public string UserName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public List<Points>? PositivePoints { get; set; }
    public List<Points>? NegativePoints { get; set; }
    public int Status { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public virtual Product? Product { get; set; }
}