namespace api_vendamode.Models.Dtos.designDto;

public class ArticleBannerUpsertDto
{
    public Guid? Id { get; set; }
    public int Index { get; set; }
    public bool IsActive { get; set; }
    public Guid? ArticleId { get; set; }
}