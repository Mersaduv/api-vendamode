namespace api_vendamode.Models.Dtos.ProductDto.Category;

public class CategoryUpdateDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public int Level { get; set; }
}