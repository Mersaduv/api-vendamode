using api_vendamode.Entities.Products;
using api_vendamode.Models.Dtos.ProductDto.Category;

namespace api_vendamode.Models.Dtos.ProductDto;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<EntityImageDto>? ImagesSrc { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public double Price { get; set; }
    public bool IsFake { get; set; }
    public int? BrandId { get; set; }
    public int InStock { get; set; }
    public ProductSizeInfo? ProductSizeInfo { get; set; }
    public ProductFeatureInfo? ProductFeatureInfo { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Discount { get; set; }
    public List<Guid?>? CategoryList { get; set; }
    public List<CategoryLevels>? CategoryLevels { get; set; }
    public Guid CategoryId { get; set; }
    public List<string>? Size { get; set; }
    public int? Sold { get; set; }
    public double? Rating { get; set; }
    public int? ReviewCount { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }
}