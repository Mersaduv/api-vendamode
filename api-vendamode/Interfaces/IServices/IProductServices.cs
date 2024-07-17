using api_vendace.Entities.Products;
using api_vendace.Mapper;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Query;

namespace api_vendace.Interfaces.IServices;

public interface IProductServices
{
    Task<ServiceResponse<bool>> CreateProduct(ProductCreateDTO productCreateDTO);
    Task<ServiceResponse<IEnumerable<Product>>> GetAll();
    Task<ServiceResponse<ProductDTO>> GetBy(string slug);
    Task<ServiceResponse<List<ProductDTO>>> GetByCategory(Guid id);
    Task<ServiceResponse<GetProductsResult>> GetProducts(RequestQuery parameters);
    Task<ServiceResponse<GetProductsResult>> GetProductsPagination(RequestQuery requestQuery);
    Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id);
    Task<ServiceResponse<bool>> UpdateProduct(ProductUpdateDTO productUpdate);
    Task<ServiceResponse<bool>> DeleteAsync(Guid id);
    Task<ServiceResponse<bool>> BulkUpdateProductStatus(List<Guid> productIds, bool isActive);
}