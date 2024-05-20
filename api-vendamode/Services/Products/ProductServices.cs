using api_vendamode.Interfaces;
using api_vendamode.Interfaces.IRepository;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Mapper;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Utility;

public class ProductServices : IProductServices
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    public ProductServices(IProductRepository productRepository, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
    }

    public async Task<ServiceResponse<bool>> Create(ProductCreateDTO productCreateDTO)
    {
        if (_productRepository.GetAsyncBy(productCreateDTO.Code).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "قبلا این محصول به ثبت رسیده"
            };
        }
        var product = productCreateDTO.ToProducts(_byteFileUtility);
        await _productRepository.CreateAsync(product);

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public Task<ServiceResponse<GetAllResponse>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id)
    {
        throw new NotImplementedException();
    }
}