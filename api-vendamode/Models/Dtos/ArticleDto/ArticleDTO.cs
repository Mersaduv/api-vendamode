using api_vendace.Models;
using api_vendace.Models.Dtos;

namespace api_vendamode.Models.Dtos.ArticleDto;

public class ArticleDto : BaseClass<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Code { get; set; } = string.Empty;
    public EntityImageDto? Image { get; set; }
    public int Place { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int NumReviews { get; set; }
    public bool IsDeleted { get; set; }
}