using System.Reflection;
using api_vendace.Entities.Products;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Models.Dtos.ProductDto.Stock;
using api_vendace.Utility;
using api_vendamode.Utility;
using Newtonsoft.Json;

namespace api_vendace.Models.Dtos.ProductDto;

public class ProductUpdateDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IFormFile MainThumbnail { get; set; } = default!;
    public List<IFormFile> Thumbnail { get; set; } = [];
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsFake { get; set; }
    public Guid? BrandId { get; set; }
    public List<Guid>? FeatureValueIds { get; set; }
    public ProductScaleDTO? ProductScale { get; set; }
    public List<StockItemDTO>? StockItems { get; set; }

    public static async ValueTask<ProductUpdateDTO?> BindAsync(HttpContext context,
                                                                 ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();
        var mainThumbnail = form.Files.GetFile("MainThumbnail");
        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var productId = Guid.Parse(form["Id"]!);
        var title = form["Title"];
        var isActive = bool.Parse(form["IsActive"]!);
        var categoryId = Guid.Parse(form["CategoryId"]!);
        var description = form["Description"];
        var isFake = bool.Parse(form["IsFake"]!);
        var brandId = string.IsNullOrEmpty(form["BrandId"]) ? null : (Guid?)Guid.Parse(form["BrandId"]!);

        List<Guid> featureValueIds = new List<Guid>();
        foreach (var id in form["FeatureValueIds"])
        {
            var idParts = id?.Split(',');
            foreach (var part in idParts!)
            {
                if (Guid.TryParse(part, out Guid guid))
                {
                    featureValueIds.Add(guid);
                }
                else
                {
                    // handle the invalid format case, for example log it or throw an exception
                    throw new FormatException($"Invalid GUID format: {part}");
                }
            }
        }

        var stockItemData = form["StockItems"].ToList();
        var stockItems = ParseHelperV2.ParseData<StockItemDTO>(stockItemData);

        var productScaleData = form["ProductScale"];
        var productScale = string.IsNullOrEmpty(productScaleData) ? null : JsonConvert.DeserializeObject<ProductScaleDTO>(productScaleData!);

        return new ProductUpdateDTO
        {
            Id = productId,
            MainThumbnail = mainThumbnail!,
            Thumbnail = thumbnail!,
            Title = title!,
            IsActive = isActive,
            CategoryId = categoryId,
            Description = description!,
            IsFake = isFake,
            BrandId = brandId,
            FeatureValueIds = featureValueIds,
            StockItems = stockItems,
            ProductScale = productScale
        };
    }
}
