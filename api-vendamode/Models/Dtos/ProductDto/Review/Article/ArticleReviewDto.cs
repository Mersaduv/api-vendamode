using api_vendace.Models;
namespace api_vendamode.Models.Dtos.ProductDto.Review.Article;
public class ArticleReviewDto : BaseClass<Guid>
{
    public string Comment { get; set; } = string.Empty;
    public int Status { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}