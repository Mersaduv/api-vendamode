using api_vendace.Entities.Products;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Models.Query;

namespace api_vendace.Interfaces.IServices;

public interface IProductSizeServices
{
    Task<ServiceResponse<bool>> AddProductSize(ProductSizeCreateDTO productSizeCreate);
    Task<ServiceResponse<bool>> UpdateProductSize(ProductSizeUpdateDTO productSizeUpdate);
    Task<ServiceResponse<bool>> AddSize(SizeCreateDTO sizeCreate);
    Task<ServiceResponse<IReadOnlyList<ProductSize>>> GetAllProductSizes();
    Task<ServiceResponse<Pagination<SizeDTO>>> GetAllSizes(RequestQuery requestQuery);
    Task<ServiceResponse<ProductSizeDTO>> GetProductSizeBy(Guid id);
    Task<ServiceResponse<Sizes>> GetSizeBy(Guid id);
    Task<ServiceResponse<ProductSizeDTO>> GetProductSizeByCategory(Guid id);
    Task<ServiceResponse<ProductSizeDTO>> GetCategoryByProductSize(Guid id);
    Task<ServiceResponse<bool>> DeleteSize(Guid id);
    Task<ServiceResponse<bool>> UpdateSize(SizeUpdateDTO sizeUpdate);
}