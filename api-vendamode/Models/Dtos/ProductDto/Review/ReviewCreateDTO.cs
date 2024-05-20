namespace api_vendamode.Models.Dtos.ProductDto.Review;

public class ReviewCreateDTO
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public List<Points>? PositivePoints { get; set; }
    public List<Points>? NegativePoints { get; set; }
    public int Status { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
}