using api_vendace.Models;

namespace api_vendamode.Entities.Designs;

public class GeneralSetting : BaseClass<Guid>
{

    public string Title { get; set; } = string.Empty;
    public string ShortIntroduction { get; set; } = string.Empty;
    public string GoogleTags { get; set; } = string.Empty;
}