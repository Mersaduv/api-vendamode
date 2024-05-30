namespace api_vendamode.Models.Dtos.ProductDto.Sizes;

public class SizeUpdateDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}