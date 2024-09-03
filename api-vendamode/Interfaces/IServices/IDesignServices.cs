using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendamode.Entities.Designs;
using api_vendamode.Entities.Products;
using api_vendamode.Models.Dtos.designDto;
using api_vendamode.Models.Dtos.ProductDto;

namespace api_vendamode.Interfaces.IServices;
public interface IDesignServices
{
    Task<ServiceResponse<bool>> UpsertHeaderText(HeaderTextUpsertDTO headerTextDto);
    Task<ServiceResponse<HeaderText>> GetHeaderText();
    Task<ServiceResponse<bool>> UpsertSupport(Support support);
    Task<ServiceResponse<Support>> GetSupport();
    Task<ServiceResponse<bool>> UpsertRedirect(Redirects redirects);
    Task<ServiceResponse<Redirects>> GetRedirect();
    Task<ServiceResponse<bool>> UpsertStoreCategory(List<StoreCategory> storeCategories);
    Task<ServiceResponse<IReadOnlyList<StoreCategory>>> GetStoreCategories();
    Task<ServiceResponse<IReadOnlyList<CategoryDTO>>> GetStoreCategoryList();
    Task<ServiceResponse<bool>> DeleteStoreCategory(Guid id);
    Task<ServiceResponse<bool>> UpsertGeneralSettings(GeneralSettingUpsertDTO settingUpsertDTO);
    Task<ServiceResponse<GeneralSetting>> GetGeneralSettings();
    Task<ServiceResponse<bool>> UpsertLogoImages(LogoUpsertDTO logoUpsertDTO);
    Task<ServiceResponse<IReadOnlyList<LogoImagesDTO>>> GetLogoImages();
    Task<ServiceResponse<bool>> UpsertSloganFooter(SloganFooter sloganFooter);
    Task<ServiceResponse<SloganFooter>> GetSloganFooters();

    Task<ServiceResponse<bool>> UpsertCopyright(Copyright copyright);
    Task<ServiceResponse<Copyright>> GetCopyright();

    Task<ServiceResponse<bool>> UpsertColumnFooters(List<ColumnFooter> columnFooters);
    Task<ServiceResponse<List<ColumnFooter>>> GetColumnFooters();

    // DesignItem
    Task<ServiceResponse<bool>> AddDesignItem(DesignItemUpsertDTO designItemUpsertDto);
    Task<ServiceResponse<bool>> UpsertDesignItems(List<DesignItemUpsertDTO> designItemUpsertDtos);
    Task<ServiceResponse<bool>> DeleteDesignItem(Guid id);
    Task<ServiceResponse<DesignItemDTO>> GetDesignItemBy(Guid id);
    Task<ServiceResponse<IReadOnlyList<DesignItemDTO>>> GetDesignItems();
}