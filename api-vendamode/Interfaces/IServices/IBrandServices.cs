using api_vendamode.Entities.Products;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Brand;
using api_vendamode.Models.Query;

namespace api_vendamode.Interfaces.IServices;

public interface IBrandServices
{
    Task<ServiceResponse<bool>> AddBrand(BrandCommandDTO brand);
    Task<ServiceResponse<bool>> UpdateBrand(BrandCommandDTO brandUpdate);
    Task<ServiceResponse<bool>> DeleteBrand(Guid id);
    Task<ServiceResponse<BrandDTO>> GetBrandBy(Guid id);
    Task<ServiceResponse<Pagination<BrandDTO>>> GetBrands(RequestQuery requestQuery);
    Task<ServiceResponse<IReadOnlyList<Brand>>> GetAllBrands();
}

