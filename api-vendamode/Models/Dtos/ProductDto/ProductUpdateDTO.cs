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
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<IFormFile> Thumbnail { get; set; } = [];
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsFake { get; set; }
    public Guid? BrandId { get; set; }
    public List<Guid>? FeatureValueIds { get; set; }
    public int InStock { get; set; }
    public int Sold { get; set; }
    public double Price { get; set; }
    public double? Discount { get; set; }
    public ProductScaleDTO? ProductScale { get; set; }
    public List<StockItemDTO>? StockItems { get; set; }

    public static async ValueTask<ProductUpdateDTO?> BindAsync(HttpContext context,
                                                                 ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
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
        var inStock = Convert.ToInt32(form["InStock"]);
        var sold = Convert.ToInt32(form["Sold"]);
        var price = Convert.ToDouble(form["Price"]);
        var discount = string.IsNullOrEmpty(form["Discount"]) ? null : (double?)Convert.ToDouble(form["Discount"]);


        var stockItemData = form["StockItems"].ToList();
        var stockItems = ParseHelper.ParseData<StockItemDTO>(stockItemData);

        var productScaleData = form["ProductScale"];
        var productScale = string.IsNullOrEmpty(productScaleData) ? null : JsonConvert.DeserializeObject<ProductScaleDTO>(productScaleData!);

        return new ProductUpdateDTO
        {
            Thumbnail = thumbnail!,
            Title = title!,
            IsActive = isActive,
            CategoryId = categoryId,
            Description = description!,
            IsFake = isFake,
            BrandId = brandId,
            FeatureValueIds = featureValueIds,
            InStock = inStock,
            Sold = sold,
            Price = price,
            Discount = discount,
            StockItems = stockItems,
            ProductScale = productScale
        };
    }
}
