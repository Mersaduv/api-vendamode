using api_vendamode.Entities.Products;
using api_vendamode.Models.DTO.ProductDto.Category;

namespace api_vendamode.Models.Dtos.ProductDto;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<EntityImageDto>? ImagesSrc { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Discount { get; set; }
    public List<int?>? CategoryList { get; set; }
    public IEnumerable<CategoryDTO>? CategoryLevels { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public List<string>? Size { get; set; }
    public List<ProductColor>? Colors { get; set; }
    public List<ProductAttributeDto>? Info { get; set; }
    public List<ProductAttributeDto>? Specifications { get; set; }
    public int InStock { get; set; }
    public int? Sold { get; set; }
    public double? Rating { get; set; }
    public int? NumReviews { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }
}