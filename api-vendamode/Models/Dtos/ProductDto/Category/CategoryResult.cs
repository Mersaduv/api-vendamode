using api_vendace.Models.Dtos.ProductDto.Category;

namespace api_vendamode.Models.Dtos.ProductDto.Category;

public class CategoryResult
{
    public List<CategoryDTO> CategoryDTO { get; set; } = default!;
    public List<CategoryDTO> CategoryList { get; set; } = default!;
}