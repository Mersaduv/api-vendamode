using api_vendace.Entities.Products;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Brand;
using api_vendace.Models.Query;

namespace api_vendace.Interfaces.IServices;

public interface IBrandServices
{
    Task<ServiceResponse<bool>> AddBrand(BrandCommandDTO brand);
    Task<ServiceResponse<bool>> UpdateBrand(BrandCommandDTO brandUpdate);
    Task<ServiceResponse<bool>> DeleteBrand(Guid id);
    Task<ServiceResponse<BrandDTO>> GetBrandBy(Guid id);
    Task<ServiceResponse<Pagination<BrandDTO>>> GetBrands(RequestQuery requestQuery);
    Task<ServiceResponse<IReadOnlyList<Brand>>> GetAllBrands();
}

