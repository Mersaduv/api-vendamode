namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class SizeIds
{
    public Guid Id { get; set; }
    public Guid SizeId { get; set; }
    public string Name { get; set; } = string.Empty;
        public Guid ProductScaleId { get; set; } // Foreign key to ProductScale
}
