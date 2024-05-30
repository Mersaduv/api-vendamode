namespace api_vendamode.Models.Dtos.ProductDto.Sizes;

public class ProductScaleDTO
{
    public List<SizeIds>? Columns { get; set; }
    public List<SizeModel>? Rows { get; set; }
    public Guid ProductId { get; set; }
}
