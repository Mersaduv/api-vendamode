using api_vendace.Models;

namespace api_vendamode.Entities.Designs;

public class FooterArticleColumn : BaseClass<Guid>
{
    public Guid ArticleId { get; set; }
    public int Index { get; set; }
}