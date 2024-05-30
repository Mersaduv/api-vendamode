using api_vendamode.Models;

namespace api_vendamode.Entities.Products;

public class Category : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public List<EntityImage<Guid, Category>>? Images { get; set; }
    public Guid? MainCategoryId { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    public virtual List<Category>? ChildCategories { get; set; }
    public virtual List<Product>? Products { get; set; }
    public virtual List<ProductFeature>? ProductFeatures { get; set; }
    public virtual ProductSize? ProductSizes { get; set; }
}
