using ApiAryanakala.Entities.Product;
using ApiAryanakala.Models;
using ApiAryanakala.Models.DTO.ProductDto;
using ApiAryanakala.Models.DTO.ProductDto.Category;

namespace ApiAryanakala.Interfaces.IServices;

public interface ICategoryService
{
    Task<ServiceResponse<CategoryDTO?>> GetBy(int? id, string? slug);
    Task<ServiceResponse<CategoryDTO>> Add(CategoryCreateDTO category);
    Task<ServiceResponse<CategoryDTO>> Update(CategoryUpdateDTO category);
    Task<ServiceResponse<bool>> Delete(int id);
    Task<ServiceResponse<bool>> UpsertCategoryImages(Thumbnails thumbnails, int id);
    Task<ServiceResponse<bool>> DeleteCategoryImages(string fileName);
    Task<Category?> GetCategoryAsyncBy(int? id , string? slug);
    Task<List<Category>> GetCategoryAsync();
    IEnumerable<int> GetAllChildCategoriesHelper(int parentCategoryId, List<Category> allCategories, List<Category> allChildCategories);
    IEnumerable<int> GetAllChildCategories(int parentCategoryId);
    Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetAllCategories(int? page, int? pageSize);

}