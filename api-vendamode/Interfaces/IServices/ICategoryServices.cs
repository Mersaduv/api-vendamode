using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Entities.Products;
using api_vendace.Models.Dtos.ProductDto.Feature;
using api_vendamode.Models.Dtos.ProductDto.Category;

namespace ApiAryanakala.Interfaces.IServices;

public interface ICategoryServices
{
    Task<ServiceResponse<CategoryDTO>> GetBy(Guid id);
    Task<ServiceResponse<CategoryDTO>> GetBy(string name);
    Task<ServiceResponse<bool>> AddCategory(CategoryCreateDTO category);
    Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetMainCategoriesAsync();
    Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetCategoriesTreeAsync();
    Task<ServiceResponse<List<CategoryDTO>>> GetAllSubCategories();
    Task<ServiceResponse<bool>> UpdateCategory(CategoryUpdateDTO category);
    Task<ServiceResponse<bool>> UpdateFeatureInCategory(CategoryFeatureUpdateDTO categoryFeatureDTO);
    Task<ServiceResponse<bool>> Delete(Guid id);
    Task<Category?> GetCategoryAsyncBy(Guid id);
    Task<List<Category>> GetCategoriesAsync();
    IEnumerable<Guid> GetAllChildCategoriesHelper(Guid parentCategoryId, List<Category> allCategories, List<Category> allChildCategories);
    IEnumerable<Guid> GetAllChildCategories(Guid parentCategoryId);
    Task<ServiceResponse<List<CategoryDTO>>> GetParentSubCategoryAsync(Guid id);
    Task<ServiceResponse<List<CategoryDTO>>> GetSubCategoryAsync(Guid id);
    Task<ServiceResponse<CategoryResult>> GetAllCategories();
}