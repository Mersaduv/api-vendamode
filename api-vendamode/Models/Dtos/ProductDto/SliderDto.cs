using api_vendace.Models;
using api_vendace.Models.Dtos;

namespace api_vendamode.Models.Dtos.ProductDto;

public class SliderDto : BaseClass<Guid>
{
    public Guid? CategoryId { get; set; }
    public EntityImageDto? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool IsMain { get; set; }
}