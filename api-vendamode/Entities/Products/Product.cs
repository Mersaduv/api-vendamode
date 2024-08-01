using api_vendace.Enums;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Models.Dtos.ProductDto.Stock;

namespace api_vendace.Entities.Products;

public class Product : BaseClass<Guid>
{
    public string Title { get; set; } = string.Empty;
    public EntityMainImage<Guid, Product> MainImage { get; set; } = new EntityMainImage<Guid, Product>();
    public List<EntityImage<Guid, Product>> Images { get; set; } = [];
    public string Code { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsFake { get; set; }
    public StatusType Status { get; set; }
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Discount { get; set; }
    public CategoryLevels? CategoryLevels { get; set; }
    public List<Guid>? FeatureValueIds { get; set; }
    public Guid? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
    public int Sold { get; set; }
    public double Rating { get; set; }
    public int NumReviews { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new List<ProductFeature>(); public virtual ProductSize? ProductSizes { get; set; }
    public Guid ProductScaleId { get; set; }
    public int InStock { get; set; }
    public virtual ProductScale? ProductScale { get; set; }
    public virtual List<Review>? Review { get; set; }
    public virtual List<StockItem>? StockItems { get; set; }
}
