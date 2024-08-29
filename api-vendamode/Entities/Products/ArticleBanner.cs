using api_vendace.Models;

namespace api_vendamode.Entities.Products;

public class ArticleBanner : BaseClass<Guid>
{
    public Guid ArticleId { get; set; }
    public int Index { get; set; }
    public bool IsActive { get; set; }
    public Article? Article { get; set; }
}