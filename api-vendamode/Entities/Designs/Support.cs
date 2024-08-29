using api_vendace.Models;

namespace api_vendamode.Entities.Designs;

public class Support : BaseClass<Guid>
{
    public string ContactAndSupport { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
}