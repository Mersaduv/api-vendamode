using api_vendace.Models;
using api_vendace.Models.Dtos;

namespace api_vendamode.Models.Dtos.ProductDto;
public class SliderDto : BaseClass<Guid>
{
    public Guid? CategoryId { get; set; }
    public EntityImageDto? Image { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
