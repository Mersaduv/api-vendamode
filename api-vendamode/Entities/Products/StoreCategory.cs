using api_vendace.Models;

namespace api_vendamode.Entities.Products;

public class StoreCategory : BaseClass<Guid>
{
    public Guid CategoryId { get; set; }
}