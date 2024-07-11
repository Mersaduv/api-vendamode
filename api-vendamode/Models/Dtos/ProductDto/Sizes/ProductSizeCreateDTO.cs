using System.Reflection;
using api_vendace.Enums;

namespace api_vendace.Models.Dtos.ProductDto.Sizes;

public class ProductSizeCreateDTO
{
    public SizeType SizeType { get; set; }
    public List<string>? ProductSizeValues { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }
    public List<Guid>? CategoryIds { get; set; }

    public static async ValueTask<ProductSizeCreateDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        
        var sizeTypeString = form["SizeType"];
        var sizeType = Enum.TryParse<SizeType>(sizeTypeString, out var parsedSizeType) ? parsedSizeType : default;
        
        var productSizeValues = form["ProductSizeValues"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        
        var categoryIdStrings = form["CategoryIds"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var categoryIds = categoryIdStrings.Select(id => Guid.Parse(id)).ToList();

        return new ProductSizeCreateDTO
        {
            SizeType = sizeType,
            Thumbnail = thumbnail,
            CategoryIds = categoryIds,
            ProductSizeValues = productSizeValues
        };
    }
}

