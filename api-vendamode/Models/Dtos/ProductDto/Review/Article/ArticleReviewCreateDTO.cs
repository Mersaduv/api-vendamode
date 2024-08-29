namespace api_vendamode.Models.Dtos.ProductDto.Review.Article;
public class ArticleReviewCreateDTO
{
    public string Comment { get; set; } = string.Empty;
    public Guid ArticleId { get; set; }
}
