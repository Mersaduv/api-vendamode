using api_vendace.Entities.Users;
using api_vendace.Models;
using api_vendamode.Entities.Products;

namespace api_vendamode.Entities;

public class ArticleReview : BaseClass<Guid>
{
    public string Comment { get; set; } = string.Empty;
    public int Status { get; set; }
    public Guid ArticleId { get; set; }
    public virtual Article Article { get; set; } = default!;
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = default!;
}
