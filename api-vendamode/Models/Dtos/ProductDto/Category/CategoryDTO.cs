using api_vendace.Models.Dtos;
using api_vendamode.Models.Dtos.ProductDto.Category;

namespace api_vendace.Models.Dtos.ProductDto.Category;

public class CategoryDTO : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public EntityImageDto? ImagesSrc { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int Level { get; set; }
    public int Count { get; set; }
    public int SubCategoryCount { get; set; }
    public int FeatureCount { get; set; }
    public List<Guid>? FeatureIds { get; set; }
    public int SizeCount { get; set; }
    public int ProductSizeCount { get; set; }
    public List<Guid>? ProductSizeId { get; set; }
    public CategorySizeDTO CategorySizes { get; set; } = new CategorySizeDTO();
    public int BrandCount { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public bool HasSizeProperty { get; set; }
    public CategoryDTO? ParentCategory { get; set; }
    public List<CategoryDTO>? ChildCategories { get; set; }
    public IEnumerable<CategoryDTO>? Categories { get; set; }
    public List<CategoryDTO> ParentCategories { get; set; }
    public List<CategoryDTO> ParentCategoriesTree { get; set; }
}


public class CategorySizeDTO
{
    public List<Guid>? Ids { get; set; }
    public List<Guid>? SizeIds { get; set; }
}
