namespace api_vendamode.Models.Dtos.ProductDto.Order;

public class OrderUpsertDTO
{
    public Guid OrderId { get; set; }
    public int Status { get; set; }
    public bool Paid { get; set; }
    public bool Delivered { get; set; }
}