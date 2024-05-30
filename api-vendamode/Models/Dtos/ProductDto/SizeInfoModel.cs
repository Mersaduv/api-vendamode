namespace api_vendamode.Models.Dtos.ProductDto;

public class SizeInfoModel
{
    public string ProductSizeValue { get; set; } = string.Empty;
    public List<string>? ScaleValues { get; set; }
}