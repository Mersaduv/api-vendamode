using System.Reflection;
using api_vendamode.Entities.Products;
using api_vendamode.Utility;

namespace api_vendamode.Models.Dtos.ProductDto;

public class ProductCreateDTO
{
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Discount { get; set; }
    public required List<IFormFile> Thumbnail { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public List<ProductColor>? Colors { get; set; }
    public List<string>? Size { get; set; }
    public List<ProductAttributeDto>? Info { get; set; }
    public List<ProductAttributeDto>? Specifications { get; set; }
    public int InStock { get; set; }
    public int? Sold { get; set; }
    // public int? NumReviews { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastUpdated { get; set; }
    public static async ValueTask<ProductCreateDTO?> BindAsync(HttpContext context,
                                                                ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var title = form["Title"];
        var code = form["Code"];
        var slug = form["Slug"];
        var price = double.Parse(form["Price"]!);
        var description = form["Description"];
        var discount = string.IsNullOrEmpty(form["Discount"]) ? null : (double?)double.Parse(form["Discount"]!);
        var categoryId = int.Parse(form["CategoryId"]!);
        var brandId = string.IsNullOrEmpty(form["BrandId"]) ? null : (int?)int.Parse(form["BrandId"]!);
        var sizes = string.IsNullOrEmpty(form["Size"]) ? null : (List<string>?)form["Size"].ToList()!;
        var colors = string.IsNullOrEmpty(form["Colors"]) ? null : (List<string>?)form["Colors"].ToList()!;
        var productColors =colors is not null ? ParseHelper.ParseData<ProductColor>(colors!) : null;
        var infoDtoData = ParseHelper.ParseData<ProductAttributeDto>(form["Info"].ToList());
        var specificationDtoData = ParseHelper.ParseData<ProductAttributeDto>(form["Specifications"].ToList());
        var inStock = int.Parse(form["InStock"]!);
        var sold = string.IsNullOrEmpty(form["Sold"]) ? null : (int?)int.Parse(form["Sold"]!);
        var created = string.IsNullOrEmpty(form["Created"]) ? null : (DateTime?)DateTime.Parse(form["Created"]!);
        var lastUpdated = string.IsNullOrEmpty(form["LastUpdated"]) ? null : (DateTime?)DateTime.Parse(form["LastUpdated"]!);

        return new ProductCreateDTO
        {
            Thumbnail = thumbnail!,
            Title = title!,
            Code = code!,
            Slug = slug!,
            Price = price,
            Description = description!,
            Discount = discount,
            CategoryId = categoryId,
            BrandId = brandId,
            Size = sizes,
            Colors = productColors,
            Info = infoDtoData,
            Specifications = specificationDtoData,
            InStock = inStock,
            Sold = sold,
            Created = created,
            LastUpdated = lastUpdated
        };
    }
}

