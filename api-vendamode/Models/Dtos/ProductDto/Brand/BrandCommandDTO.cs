using System.Reflection;

namespace api_vendace.Models.Dtos.ProductDto.Brand;

public class BrandCommandDTO
{
    public Guid? Id { get; set; }
    public string NameFa { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool InSlider { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }

    public static async ValueTask<BrandCommandDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var id = string.IsNullOrEmpty(form["Id"]) ? null : (Guid?)Guid.Parse(form["Id"]!);
        var nameFa = form["NameFa"];
        var nameEn = form["NameEn"];
        var description = form["Description"];
        var isActive = bool.Parse(form["IsActive"]!);
        var inSlider = bool.Parse(form["InSlider"]!);
        return new BrandCommandDTO
        {
            Id = id,
            Thumbnail = thumbnail,
            NameFa = nameFa!,
            NameEn = nameEn!,
            Description = description!,
            IsActive = isActive,
            InSlider = inSlider
        };
    }
}