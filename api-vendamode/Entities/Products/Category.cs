using api_vendamode.Models;

namespace api_vendamode.Entities.Products;

public class Category : BaseClass<int>
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Level { get; set; }
    public ProductColor? Colors { get; set; }
    public List<EntityImage<int, Category>>? Images { get; set; }
    public int? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    public virtual List<Category>? ChildCategories { get; set; }
    public virtual List<Product>? Products { get; private set; }
}
