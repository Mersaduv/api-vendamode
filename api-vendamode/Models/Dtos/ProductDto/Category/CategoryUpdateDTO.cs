namespace api_vendamode.Models.Dtos.ProductDto.Category;

public class CategoryUpdateDTO : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public List<IFormFile>? Thumbnail { get; set; }
    public Guid? MainCategoryId { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
}