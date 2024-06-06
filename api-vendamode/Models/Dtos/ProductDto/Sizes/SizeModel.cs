namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class SizeModel
{
    public string Id { get; set; }= string.Empty;
    public List<string>? ScaleValues { get; set; }
    public string ProductSizeValueName { get; set; } = string.Empty;
    public string ProductSizeValueId { get; set; }= string.Empty;
}