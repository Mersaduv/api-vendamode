using api_vendace.Models;

namespace api_vendamode.Models.Dtos.ProductDto.Order;

public class OrderResult
{
    public int OrdersLength { get; set; }
    public Pagination<OrderDTO> Pagination { get; set; } = default!;
}