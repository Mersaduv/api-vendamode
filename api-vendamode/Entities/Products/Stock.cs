
using api_vendace.Models;

namespace api_vendace.Entities.Products;

public class Stock : BaseClass<Guid>
{
    public Guid ProductId { get; set; }
    public int Count { get; set; }
}