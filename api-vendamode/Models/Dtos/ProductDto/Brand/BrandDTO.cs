namespace api_vendamode.Models.Dtos.ProductDto.Brand;

public class BrandDTO : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
     public EntityImageDto? ImagesSrc { get; set; }
    public bool InSlider { get; set; }
    public bool IsActive { get; set; }
    public int Count { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDelete { get; set; }
}