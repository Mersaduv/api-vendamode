using api_vendace.Entities.Products;
using api_vendace.Enums;
using api_vendace.Models.Dtos.ProductDto.Brand;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Models.Dtos.ProductDto.Stock;
using api_vendamode.Models.Dtos.ProductDto.Category;
using api_vendamode.Models.Dtos.ProductDto.Stock;

namespace api_vendace.Models.Dtos.ProductDto;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public EntityImageDto? MainImageSrc { get; set; }
    public List<EntityImageDto>? ImagesSrc { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public double Price { get; set; }
    public StatusType Status { get; set; }
    public bool IsFake { get; set; }
    public bool IsActive { get; set; }
    public Guid? BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public BrandDTO? BrandData { get; set; }
    public int InStock { get; set; }
    public ProductSizeInfo? ProductSizeInfo { get; set; }
    public virtual ProductScaleDTO? ProductScale { get; set; }
    public ProductFeatureInfo? ProductFeatureInfo { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Discount { get; set; }
    public List<Guid?>? CategoryList { get; set; }
    public List<CategoryLevels>? CategoryLevels { get; set; }
    public CategoryWithAllParents ParentCategories { get; set; }
    public List<GetStockItemDTO> StockItems { get; set; }
    public Guid CategoryId { get; set; }
    public List<string>? Size { get; set; }
    public int? Sold { get; set; }
    public double? Rating { get; set; }
    public int? ReviewCount { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }
}