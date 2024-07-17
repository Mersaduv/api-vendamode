namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class SizeModel
{
    public string Id { get; set; } = string.Empty;
    public string Idx { get; set; } = string.Empty;
    public string? ModelSizeId { get; set; }
    public List<string>? ScaleValues { get; set; }
    public Guid ProductScaleId { get; set; } // Foreign key to ProductScale

    public string ProductSizeValue { get; set; } = string.Empty;
    public string ProductSizeValueId { get; set; } = string.Empty;
}