using System.Reflection;
using api_vendamode.Utility;

namespace api_vendamode.Models.Dtos.ProductDto.Order;

public class OrderReturnedDTO
{
    public Guid OrderId { get; set; }
    public Guid ReturnedId { get; set; }
    public List<CartItem>? Items { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<IFormFile>? Thumbnail { get; set; }

    public static async ValueTask<OrderReturnedDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var orderId = Guid.TryParse(form["OrderId"], out var orderIdParsed) ? orderIdParsed : Guid.Empty;
        var returnedId = Guid.TryParse(form["ReturnedId"], out var returnedIdParsed) ? returnedIdParsed : Guid.Empty;
        var description = form["Description"];
        var thumbnailFiles = form.Files.GetFiles("Thumbnail").ToList();

        var itemIdForm = form["Items"];
        var itemIdData = itemIdForm.ToList();
        var itemID = ParseHelper.ParseData<CartItem>(itemIdData!);

        if (itemID == null)
        {
            return null;
        }

        return new OrderReturnedDTO
        {
            OrderId = orderId,
            ReturnedId = returnedId,
            Items = itemID,
            Description = description!,
            Thumbnail = thumbnailFiles
        };
    }
}

public class CartItem
{
    public string Id { get; set; } = string.Empty;
    public int Quantity { get; set; }
}