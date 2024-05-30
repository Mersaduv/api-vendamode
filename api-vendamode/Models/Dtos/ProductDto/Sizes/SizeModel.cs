namespace api_vendamode.Models.Dtos.ProductDto.Sizes;

public class SizeModel
{
    public Guid ProductSizeValueId { get; set; }
    public List<string>? ScaleValues { get; set; }
}