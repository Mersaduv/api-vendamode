using api_vendamode.Entities.Products;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Models.Dtos.ProductDto.Sizes;

namespace api_vendamode.Interfaces.IServices;

public interface IProductSizeServices
{
    Task<ServiceResponse<bool>> AddProductSize(ProductSizeCreateDTO productSizeCreate);
    Task<ServiceResponse<bool>> AddSize(SizeCreateDTO sizeCreate);
    Task<ServiceResponse<IReadOnlyList<ProductSize>>> GetAllProductSizes();
    Task<ServiceResponse<IReadOnlyList<SizeDTO>>> GetAllSizes();
    Task<ServiceResponse<ProductSizeDTO>> GetProductSizeBy(Guid id);
    Task<ServiceResponse<Sizes>> GetSizeBy(Guid id);
    Task<ServiceResponse<ProductSizeDTO>> GetProductSizeByCategory(Guid id);
    Task<ServiceResponse<bool>> DeleteSize(Guid id);
    Task<ServiceResponse<bool>> UpdateSize(SizeUpdateDTO sizeUpdate);
}