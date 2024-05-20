namespace api_vendamode.Models.Dtos.ProductDto.Review;

public class ReviewDto : BaseClass<Guid>
{
    public string Comment { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public List<Points>? PositivePoints { get; set; }
    public List<Points>? NegativePoints { get; set; }
    public int Status { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}