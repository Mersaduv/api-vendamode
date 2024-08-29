using api_vendace.Entities;
using api_vendace.Entities.Products;
using api_vendace.Models;

namespace api_vendamode.Entities.Products;

public class Article : BaseClass<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Code { get; set; } = string.Empty;
    public EntityImage<Guid, Article>? Image { get; set; }
    public int Place { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public Guid? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public virtual List<ArticleReview>? ArticleReviews { get; set; }

}