using api_vendace.Models.Dtos.ProductDto.Category;

namespace api_vendamode.Models.Dtos.ProductDto.Category;

public class CategoryWithAllParents
{
    public CategoryDTO Category { get; set; } = default!;
    public List<CategoryDTO> ParentCategories { get; set; } = default!;
}