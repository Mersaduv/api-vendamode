using System.Reflection;
using api_vendace.Enums;

namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class ProductSizeCreateDTO
{
    public SizeType SizeType { get; set; }
    public List<string>? ProductSizeValues { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? ProductId { get; set; }

    public static async ValueTask<ProductSizeCreateDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        
        var sizeTypeString = form["SizeType"];
        var sizeType = Enum.TryParse<SizeType>(sizeTypeString, out var parsedSizeType) ? parsedSizeType : default;
        
        var productSizeValues = form["ProductSizeValues"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        
        var categoryId = Guid.Parse(form["CategoryId"]!);
        var productIdString = form["ProductId"];
        var productId = string.IsNullOrEmpty(productIdString) ? null : (Guid?)Guid.Parse(productIdString!);

        return new ProductSizeCreateDTO
        {
            SizeType = sizeType,
            Thumbnail = thumbnail,
            CategoryId = categoryId,
            ProductId = productId,
            ProductSizeValues = productSizeValues
        };
    }
}

