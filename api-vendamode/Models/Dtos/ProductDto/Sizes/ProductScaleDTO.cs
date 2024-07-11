using api_vendamode.Models.Dtos.ProductDto.Sizes;

namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class ProductScaleDTO
{
    public List<SizeIdsDTO>? ColumnSizes { get; set; }
    public List<SizeModel>? Rows { get; set; }
    public Guid ProductId { get; set; }
}
