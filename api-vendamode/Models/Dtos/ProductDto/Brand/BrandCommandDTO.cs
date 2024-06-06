using System.Reflection;

namespace api_vendace.Models.Dtos.ProductDto.Brand;

public class BrandCommandDTO
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool InSlider { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }

    public static async ValueTask<BrandCommandDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var id = form["Id"];
        var name = form["Name"];
        var description = form["Description"];
        var isActive = bool.Parse(form["IsActive"]!);
        var inSlider = bool.Parse(form["InSlider"]!);
        Guid? parsedId = null;
        if (Guid.TryParse(id, out var guidId))
        {
            parsedId = guidId;
        }

        return new BrandCommandDTO
        {
            Id = parsedId,
            Thumbnail = thumbnail,
            Name = name!,
            Description = description!,
            IsActive = isActive,
            InSlider = inSlider
        };
    }
}