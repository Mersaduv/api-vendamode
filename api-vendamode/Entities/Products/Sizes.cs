using api_vendace.Models;
using api_vendamode.Entities.Products;

namespace api_vendace.Entities.Products;

public class Sizes : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<CategorySize>? CategorySizes { get; set; }

}