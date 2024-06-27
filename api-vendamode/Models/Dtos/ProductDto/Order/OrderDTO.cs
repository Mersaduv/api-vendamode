using api_vendace.Entities;
using api_vendace.Entities.Users;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.AuthDto;
using api_vendamode.Entities;
using api_vendamode.Entities.Products;

namespace api_vendamode.Models.Dtos.ProductDto.Order;

public class OrderDTO : BaseClass<Guid>
{
    public string OrderNum { get; set; } = string.Empty;
    public int Status { get; set; }
    public Guid UserID { get; set; }
    public UserDTO User { get; set; } = default!;
    public Address Address { get; set; } = default!;
    public List<Cart> Cart { get; set; } = default!;
    public Canceled? Canceled { get; set; } 
    public int TotalItems { get; set; }
    public double TotalPrice { get; set; }
    public double TotalDiscount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public bool Delivered { get; set; }
    public bool Paid { get; set; }
    public EntityImageDto? PurchaseInvoice { get; set; }
    public DateTime DateOfPayment { get; set; }
}