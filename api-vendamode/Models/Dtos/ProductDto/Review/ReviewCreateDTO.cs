using api_vendamode.Entities.Products;

namespace api_vendamode.Models.Dtos.ProductDto.Review;

public class ReviewCreateDTO
{
    public Guid UserId { get; set; }
    public List<IFormFile> ProductThumbnails { get; set; } = [];
    public List<Points>? PositivePoints { get; set; }
    public List<Points>? NegativePoints { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
}