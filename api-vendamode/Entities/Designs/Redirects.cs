using api_vendace.Models;

namespace api_vendamode.Entities.Designs;

public class Redirects : BaseClass<Guid>
{
    public Guid ArticleId { get; set; }
    public string ArticleTitle { get; set; } = string.Empty;
}