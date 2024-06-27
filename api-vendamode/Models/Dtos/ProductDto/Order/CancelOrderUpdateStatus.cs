namespace api_vendamode.Models.Dtos.ProductDto.Order;

public class CancelOrderUpdateStatus
{
    public int Status { get; set; }
    public Guid OrderId { get; set; }
    public List<string>? ItemID { get; set; }
    public Guid CanceledId { get; set; }
}