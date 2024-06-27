using api_vendace.Entities;
using api_vendace.Entities.Users;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Entities.Products;

public class Order : BaseClass<Guid>
{
    public string OrderNum { get; set; } = string.Empty;
    public int Status { get; set; }
    public Guid UserID { get; set; }
    public User User { get; set; } = default!;
    public Guid AddressId { get; set; }
    public Address Address { get; set; } = default!;
    public List<Cart> Cart { get; set; } = new List<Cart>();
    public Guid? CanceledId { get; set; }
    public Canceled? Canceled { get; set; }
    public Guid? ReturnedId { get; set; }
    public Returned? Returned { get; set; }
    public int TotalItems { get; set; }
    public double TotalPrice { get; set; }
    public double OrgPrice { get; set; }
    public double TotalDiscount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public bool Delivered { get; set; }
    public bool Paid { get; set; }
    public EntityImage<Guid, Order>? PurchaseInvoice { get; set; }
    public DateTime DateOfPayment { get; set; }
    public DateTime Updated { get; set; }
}

public class Cart
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string ItemID { get; set; } = string.Empty;
    public Guid ProductID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public double Price { get; set; }
    public double Discount { get; set; }
    public int InStock { get; set; }
    public int Sold { get; set; }
    public ColorDTO? Color { get; set; }
    public SizeDTO? Size { get; set; }
    public ObjectValue? Features { get; set; }
    public EntityImageDto? Img { get; set; }
    public int Quantity { get; set; }
}
