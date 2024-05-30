using api_vendamode.Entities.Products;
using api_vendamode.Mapper;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto;

namespace api_vendamode.Interfaces.IServices;

public interface IProductServices
{
    Task<ServiceResponse<bool>> CreateProduct(ProductCreateDTO productCreateDTO);
    Task<ServiceResponse<IEnumerable<Product>>> GetAll();
    Task<ServiceResponse<Pagination<ProductDTO>>> GetProductsPagination(int pageNumber, int pageSize);
    Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id);
    Task<ServiceResponse<bool>> UpdateProduct(Guid id);

}