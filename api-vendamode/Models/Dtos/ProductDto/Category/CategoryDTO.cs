using api_vendamode.Models.Dtos;

namespace api_vendamode.Models.Dtos.ProductDto.Category;

public class CategoryDTO : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public EntityImageDto? ImagesSrc { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int Level { get; set; }
    public int Count { get; set; }
    public int SubCategoryCount { get; set; }
    public int FeatureCount { get; set; }
    public int SizeCount { get; set; }
    public int BrandCount { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public CategoryDTO? ParentCategory { get; set; }
    public List<CategoryDTO>? ChildCategories { get; set; }
    public List<CategoryDTO>? Categories { get; set; }
}
