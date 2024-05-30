namespace api_vendamode.Models.Dtos.ProductDto;

public class ColorDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HexCode { get; set; } = string.Empty;
}