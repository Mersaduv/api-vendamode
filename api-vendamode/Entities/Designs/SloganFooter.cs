using api_vendace.Models;

namespace api_vendamode.Entities.Designs;


public class SloganFooter : BaseClass<Guid>
{
    public string Headline { get; set; } = string.Empty;
    public string IntroductionText { get; set; } = string.Empty;
}