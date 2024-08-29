using api_vendace.Models;
using api_vendace.Models.Dtos;

namespace api_vendamode.Models.Dtos.designDto;

public class ArticleBannerDto : BaseClass<Guid>
{
    public Guid ArticleId { get; set; }
    public int Index { get; set; }
    public bool IsActive { get; set; }
    public string Title { get; set; } = string.Empty;
    public EntityImageDto? ImagesSrc { get; set; }
}

