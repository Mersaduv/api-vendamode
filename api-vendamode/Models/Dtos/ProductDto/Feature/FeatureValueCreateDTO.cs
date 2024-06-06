namespace api_vendace.Models.Dtos.ProductDto.Feature;

public class FeatureValueCreateDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? HexCode { get; set; }
    public Guid ProductFeatureId { get; set; }
}