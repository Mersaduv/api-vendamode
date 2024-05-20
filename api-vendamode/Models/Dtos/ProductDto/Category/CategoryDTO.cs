using api_vendamode.Models.Dtos;

namespace api_vendamode.Models.DTO.ProductDto.Category;

public class CategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<EntityImageDto>? ImagesSrc { get; set; }
    public int? ParentCategoryId { get; set; }
    public int Level { get; set; }
    public CategoryDTO? ParentCategory { get; set; }
    public List<CategoryDTO>? ChildCategories { get; set; }
}
