using api_vendace.Models;

namespace api_vendamode.Entities.Products;

public class HeaderText : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}