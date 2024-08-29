using api_vendace.Models;
using api_vendace.Models.Dtos;

namespace api_vendamode.Models.Dtos.designDto;

public class LogoImagesDTO : BaseClass<Guid>
{
    public EntityImageDto? OrgImage { get; set; }
    public EntityImageDto? FaviconImage { get; set; }
}