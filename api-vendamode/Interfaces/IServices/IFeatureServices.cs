
using api_vendace.Entities.Products;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Feature;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos;
using api_vendamode.Models.Dtos.ProductDto;

namespace api_vendace.Interfaces.IServices;

public interface IFeatureServices
{
    Task<ServiceResponse<bool>> AddFeature(ProductFeatureCreateDTO feature);
    Task<ServiceResponse<bool>> AddFeatureValue(FeatureValueCreateDTO feartureValue);
    Task<ServiceResponse<bool>> UpdateFeature(ProductFeatureUpdateDTO feature);
    Task<ServiceResponse<bool>> UpdateFeatureValue(ProductFeatureUpdateDTO feature);
    Task<ServiceResponse<bool>> DeleteFeature(Guid id);
    Task<ServiceResponse<bool>> DeleteFeatureValue(Guid id);
    Task<ServiceResponse<ProductFeature>> GetFeatureBy(Guid id);
    Task<ServiceResponse<FeatureValue>> GetFeatureValueBy(Guid id);
    Task<ServiceResponse<GetCategoryFeaturesByCategory>> GetFeaturesByCategory(Guid id);
    Task<ServiceResponse<Pagination<ProductFeatureDto>>> GetAllFeatures(RequestQuery requestQuery);
    Task<ServiceResponse<Pagination<FeatureValue>>> GetAllFeatureValues(RequestQuery requestQuery);
}