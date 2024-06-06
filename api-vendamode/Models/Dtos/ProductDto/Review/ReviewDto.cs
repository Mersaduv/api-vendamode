using api_vendace.Entities.Products;

namespace api_vendace.Models.Dtos.ProductDto.Review;

public class ReviewDto : BaseClass<Guid>
{
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public List<Points>? PositivePoints { get; set; }
    public List<Points>? NegativePoints { get; set; }
    public List<EntityImageDto>? ImageUrls { get; set; }
    public EntityImageDto? ProductImageUrl { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}