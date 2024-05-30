using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Category;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Entities.Products;

namespace ApiAryanakala.Interfaces.IServices;

public interface ICategoryServices
{
    Task<ServiceResponse<CategoryDTO>> GetBy(Guid id);
    Task<ServiceResponse<CategoryDTO?>> GetBy(string name);
    Task<ServiceResponse<bool>> AddCategory(CategoryCreateDTO category);
    Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetMainCategoriesAsync();
    Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetCategoriesTreeAsync();
    Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetAllSubCategories();
    Task<ServiceResponse<bool>> UpdateCategory(CategoryUpdateDTO category);
    Task<ServiceResponse<bool>> Delete(Guid id);
    Task<Category?> GetCategoryAsyncBy(Guid id);
    Task<List<Category>> GetCategoriesAsync();
    IEnumerable<Guid> GetAllChildCategoriesHelper(Guid parentCategoryId, List<Category> allCategories, List<Category> allChildCategories);
    IEnumerable<Guid> GetAllChildCategories(Guid parentCategoryId);
    Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetAllCategories(int? page, int? pageSize);
}