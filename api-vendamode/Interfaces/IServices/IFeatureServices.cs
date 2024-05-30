
using api_vendamode.Entities.Products;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Feature;

namespace api_vendamode.Interfaces.IServices;

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
    Task<ServiceResponse<List<ProductFeature>>> GetFeaturesByCategory(Guid id);
    Task<ServiceResponse<IReadOnlyList<ProductFeature>>> GetAllFeatures();
    Task<ServiceResponse<IReadOnlyList<FeatureValue>>> GetAllFeatureValues();
}