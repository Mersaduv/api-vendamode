using api_vendamode.Entities.Products;
using api_vendamode.Mapper;
using api_vendamode.Models;
using api_vendamode.Models.DTO.ProductDto;
using api_vendamode.Models.Dtos.ProductDto;

namespace api_vendamode.Interfaces.IServices;

public interface IProductServices
{
    Task<ServiceResponse<bool>> Create(ProductCreateDTO productCreateDTO);
    Task<ServiceResponse<GetAllResponse>> GetAll();
    Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id);
}