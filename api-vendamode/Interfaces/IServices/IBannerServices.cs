using api_vendace.Models;
using api_vendamode.Models.Dtos.designDto;

namespace api_vendamode.Interfaces.IServices;
public interface IBannerServices
{
    Task<ServiceResponse<bool>> AddBanner(BannerCreateDto banner);
    Task<ServiceResponse<bool>> UpsertBanners(List<BannerUpsertDto> bannerUpsertDtos);
    Task<ServiceResponse<bool>> UpsertFooterBanner(FooterBannerUpsertDto bannerUpsertDto);
    Task<ServiceResponse<bool>> UpsertArticleBanners(List<ArticleBannerUpsertDto> articleBanners);
    Task<ServiceResponse<bool>> UpdateBanner(BannerUpsertDto banner);
    Task<ServiceResponse<bool>> DeleteBanner(Guid id);
    Task<ServiceResponse<BannerDto>> GetBannerBy(Guid id);
    Task<ServiceResponse<IReadOnlyList<BannerDto>>> GetBanners();
    Task<ServiceResponse<IReadOnlyList<ArticleBannerDto>>> GetArticleBanners();
    Task<ServiceResponse<IReadOnlyList<FooterBannerDto>>> GetFooterBanners();
}