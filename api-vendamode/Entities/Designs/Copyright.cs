using api_vendace.Models;

namespace api_vendamode.Entities.Designs;

public class Copyright : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
}