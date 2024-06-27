namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class SizeDTO : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}