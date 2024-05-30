using api_vendamode.Entities.Products;

namespace api_vendamode.Models.Dtos.ProductDto.Stock;

public class StockCreateDTO
{
    public List<IFormFile>? Thumbnail { get; set; }
    public string StockCode { get; set; } = string.Empty;
    public List<Guid>? ProductFeatureIds { get; set; }
    public List<Guid>? FeatureValueIds { get; set; }
    public int ProductPrice { get; set; }
    public int Discount { get; set; }
    public bool IsDeleted { get; set; }
}