using api_vendamode.Models.Dtos.ProductDto.Sizes;

namespace api_vendamode.Models.Dtos.ProductDto;

public class ProductSizeInfo
{
    public List<SizeDTO>? Columns { get; set; }
    public List<SizeInfoModel>? Rows { get; set; }
    public EntityImageDto? ImagesSrc { get; set; }
}
