using api_vendace.Entities;
using api_vendace.Models;

namespace api_vendamode.Entities.Designs;

public class LogoImages : BaseClass<Guid>
{
    public EntityMainImage<Guid, LogoImages>? OrgThumbnail { get; set; }
    public EntityImage<Guid, LogoImages>? FaviconThumbnail { get; set; }
}