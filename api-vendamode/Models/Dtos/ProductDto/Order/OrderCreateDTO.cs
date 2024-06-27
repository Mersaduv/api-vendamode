using System.Reflection;
using api_vendace.Entities.Users;
using api_vendamode.Entities.Products;
using api_vendamode.Utility;

namespace api_vendamode.Models.Dtos.ProductDto.Order;

public class OrderCreateDTO
{
    public int Status { get; set; }
    public string OrderNum { get; set; } = string.Empty;
    public Guid Address { get; set; }
    public List<Cart> Cart { get; set; } = default!;
    public Guid CanceledId { get; set; }
    public int TotalItems { get; set; }
    public double TotalPrice { get; set; }
    public double OrgPrice { get; set; }
    public double TotalDiscount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public bool Delivered { get; set; }
    public bool Paid { get; set; }
    public IFormFile? Thumbnail { get; set; }


    public static async ValueTask<OrderCreateDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFile = form.Files.GetFile("Thumbnail");
        var status = int.TryParse(form["Status"], out var statusParsed) ? statusParsed : 0;
        var orderNum = form["OrderNum"];
        var totalItems = int.TryParse(form["TotalItems"], out var totalItemsParsed) ? totalItemsParsed : 0;
        var totalPrice = double.TryParse(form["TotalPrice"], out var totalPriceParsed) ? totalPriceParsed : 0.0;
        var orgPrice = double.TryParse(form["OrgPrice"], out var orgPriceParsed) ? orgPriceParsed : 0.0;
        var totalDiscount = double.TryParse(form["TotalDiscount"], out var totalDiscountParsed) ? totalDiscountParsed : 0.0;
        var delivered = bool.TryParse(form["Delivered"], out var deliveredParsed) && deliveredParsed;
        var paid = bool.TryParse(form["Paid"], out var paidParsed) && paidParsed;
        var canceledOrder = Guid.TryParse(form["CanceledId"], out var canceledIdParsed) ? canceledIdParsed : Guid.Empty;
        var paymentMethod = form["PaymentMethod"];
        var addressId = Guid.TryParse(form["Address"], out var addressIdParsed) ? addressIdParsed : Guid.Empty;
        var cartForm = form["cart"];
        var cartData = cartForm.ToString().Split(';').ToList();
        var cart = ParseHelperV2.ParseData<Cart>(cartData!);

        if (cart == null)
        {
            return null;
        }

        return new OrderCreateDTO
        {
            Status = status,
            OrderNum = orderNum!,
            OrgPrice = orgPrice,
            Address = addressId,
            Cart = cart,
            CanceledId = canceledOrder,
            TotalItems = totalItems,
            TotalPrice = totalPrice,
            TotalDiscount = totalDiscount,
            PaymentMethod = paymentMethod!,
            Delivered = delivered,
            Paid = paid,
            Thumbnail = thumbnailFile
        };
    }
}