using Microsoft.EntityFrameworkCore;

namespace api_vendace.Models.Dtos.ProductDto;
[Owned]
public class ColorDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HexCode { get; set; } = string.Empty;
}