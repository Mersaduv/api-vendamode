using System.Reflection;
using api_vendace.Entities.Products;
using api_vendace.Enums;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Models.Dtos.ProductDto.Stock;
using api_vendace.Utility;
using api_vendamode.Utility;
using Newtonsoft.Json;

namespace api_vendace.Models.Dtos.ProductDto;

public class ProductCreateDTO
{
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IFormFile MainThumbnail { get; set; } = default!;
    public List<IFormFile> Thumbnail { get; set; } = [];
    public StatusType Status { get; set; }
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsFake { get; set; }
    public Guid? BrandId { get; set; }
    public List<Guid>? FeatureValueIds { get; set; }
    public ProductScaleDTO? ProductScale { get; set; }
    public List<StockItemDTO>? StockItems { get; set; }

    public static async ValueTask<ProductCreateDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();
        var mainThumbnail = form.Files.GetFile("MainThumbnail");
        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var title = form["Title"];
        var isActive = bool.Parse(form["IsActive"]!);
        var categoryId = Guid.Parse(form["CategoryId"]!);
        var description = form["Description"];
        var isFake = bool.Parse(form["IsFake"]!);
        var brandId = string.IsNullOrEmpty(form["BrandId"]) ? null : (Guid?)Guid.Parse(form["BrandId"]!);
        var status = Enum.Parse<StatusType>(form["Status"]!);
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
                    throw new FormatException($"Invalid GUID format: {part}");
                }
            }
        }

        var productScaleData = form["ProductScale"];
        var productScale = string.IsNullOrEmpty(productScaleData) ? null : JsonConvert.DeserializeObject<ProductScaleDTO>(productScaleData!);

        var stockItemsData = form["StockItems"];
        List<StockItemDTO> stockItems = new List<StockItemDTO>();
        if (!string.IsNullOrEmpty(stockItemsData))
        {
            var stockItemsTempList = JsonConvert.DeserializeObject<List<StockItemTempDTO>>(stockItemsData);
            foreach (var tempItem in stockItemsTempList!)
            {
                var thumbnailStock = form.Files.GetFile($"ImageStock_{tempItem.StockId}");
                var stockItem = new StockItemDTO
                {
                    StockId = tempItem.StockId,
                    Idx = tempItem.Idx,
                    ImageStock = thumbnailStock,
                    FeatureValueId = tempItem.FeatureValueId,
                    SizeId = tempItem.SizeId,
                    Quantity = tempItem.Quantity ?? 0,
                    Price = tempItem.Price ?? 0,
                    Discount = tempItem.Discount ?? 0,
                    AdditionalProperties = tempItem.AdditionalProperties
                };
                stockItems.Add(stockItem);
            }
        }

        return new ProductCreateDTO
        {
            MainThumbnail = mainThumbnail!,
            Thumbnail = thumbnail!,
            Title = title!,
            IsActive = isActive,
            CategoryId = categoryId,
            Description = description!,
            IsFake = isFake,
            Status = status,
            BrandId = brandId,
            FeatureValueIds = featureValueIds,
            StockItems = stockItems,
            ProductScale = productScale
        };
    }
}
public class StockItemTempDTO
{
    public Guid Id { get; set; }
    public int StockId { get; set; }
    public string? Idx { get; set; }
    public List<Guid>? FeatureValueId { get; set; }
    public Guid? SizeId { get; set; }
    public int? Quantity { get; set; }
    public double? Price { get; set; }
    public double? Discount { get; set; }
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}