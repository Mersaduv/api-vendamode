namespace api_vendamode.Models.Dtos.ProductDto;

public class BulkUpdateProductDTO
{
    public List<Guid> ProductIds { get; set; } = new List<Guid>();
    public bool IsActive { get; set; }
}