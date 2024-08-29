namespace api_vendamode.Models.Dtos.designDto;

public class ArticleBannerBulkUpsertDto
{
   public List<ArticleBannerUpsertDto> ArticleBanners { get; set; } = new List<ArticleBannerUpsertDto>();
}