using api_vendace.Entities.Products;
using api_vendace.Mapper;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Query;
using api_vendamode.Entities.Products;
using api_vendamode.Models.Dtos;

namespace api_vendace.Interfaces.IServices;

public interface IProductServices
{
    Task<ServiceResponse<Guid>> CreateProduct(ProductCreateDTO productCreateDTO);
    Task<ServiceResponse<string>> UploadMedia(Description file);
    Task<ServiceResponse<IEnumerable<Product>>> GetAll();
    Task<ServiceResponse<ProductDTO>> GetBy(string slug);
    Task<ServiceResponse<List<ProductDTO>>> GetByCategory(Guid id);
    Task<ServiceResponse<GetProductsResult>> GetProducts(RequestQuery parameters);
    Task<ServiceResponse<GetProductsResult>> GetProductsPagination(RequestQuery requestQuery);
    Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id);
    Task<ServiceResponse<Guid>> UpdateProduct(ProductUpdateDTO productUpdate);
    Task<ServiceResponse<bool>> DeleteAsync(Guid id);
    Task<ServiceResponse<bool>> DeleteTrashAsync(Guid id);
    Task<ServiceResponse<bool>> RestoreProductAsync(Guid id);
    Task<ServiceResponse<bool>> BulkUpdateProductStatus(List<Guid> productIds, string action);
}