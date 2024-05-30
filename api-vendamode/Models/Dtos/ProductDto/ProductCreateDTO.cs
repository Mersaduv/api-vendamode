using System.Reflection;
using api_vendamode.Entities.Products;
using api_vendamode.Models.Dtos.ProductDto.Sizes;
using api_vendamode.Models.Dtos.ProductDto.Stock;
using api_vendamode.Utility;

namespace api_vendamode.Models.Dtos.ProductDto;

public class ProductCreateDTO
{
    public string Title { get; set; } = string.Empty;//  checked
    public bool IsActive { get; set; } //  checked
    public required List<IFormFile> Thumbnail { get; set; }//  checked
    public Guid CategoryId { get; set; } //  checked
    public string Description { get; set; } = string.Empty;//  checked
    public bool IsFake { get; set; } // checked
    public int? BrandId { get; set; } // checked 
    public List<Guid>? FeatureIds { get; set; }// checked 
    public List<Guid>? FeatureValueIds { get; set; }// checked 
    public int InStock { get; set; }// checked 
    public double Price { get; set; }// checked 
    public double? Discount { get; set; }// checked 
    public ProductScaleDTO? ProductScale { get; set; } // checked
    public List<StockItemDTO> StockItems { get; set; } = new List<StockItemDTO>();
}
