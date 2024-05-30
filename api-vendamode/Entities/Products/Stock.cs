
using api_vendamode.Models;

namespace api_vendamode.Entities.Products;

public class Stock : BaseClass<Guid>
{
    public Guid ProductId { get; set; }
    public int Count { get; set; }
}