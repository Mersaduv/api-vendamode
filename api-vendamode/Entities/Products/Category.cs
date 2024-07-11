using api_vendace.Data;
using api_vendace.Models;
using api_vendamode.Entities.Products;

namespace api_vendace.Entities.Products;

public class Category : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
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
    public virtual List<CategoryProductSize>? CategoryProductSizes { get; set; }
    public virtual List<CategorySize>? CategorySizes { get; set; }
    public List<Category> GetParentCategories(ApplicationDbContext context)
    {
        List<Category> parents = new List<Category>();
        Category? current = this.ParentCategory;
        while (current != null)
        {
            context.Entry(current).Reference(c => c.ParentCategory).Load();
            parents.Add(current);
            current = current.ParentCategory;
        }
        return parents;
    }
}
