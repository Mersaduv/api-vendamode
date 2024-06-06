namespace api_vendace.Models.Dtos.ProductDto.Category;
public class CategoryLevels
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<EntityImageDto>? ImagesSrc { get; set; }
    public int? ParentCategoryId { get; set; }
    public int Level { get; set; }
}
