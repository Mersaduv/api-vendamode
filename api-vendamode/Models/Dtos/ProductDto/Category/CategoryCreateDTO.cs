using System.Reflection;

namespace api_vendace.Models.Dtos.ProductDto.Category;

public class CategoryCreateDTO
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }
    public Guid? MainCategoryId { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int Level { get; set; }

    public static async ValueTask<CategoryCreateDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var name = form["Name"];
        var slug = form["Slug"];
        var isActive = bool.Parse(form["IsActive"]!);
        var mainCategoryId = string.IsNullOrEmpty(form["MainCategoryId"]) ? null : (Guid?)Guid.Parse(form["MainCategoryId"]!);
        var parentCategoryId = string.IsNullOrEmpty(form["ParentCategoryId"]) ? null : (Guid?)Guid.Parse(form["ParentCategoryId"]!);
        var level = string.IsNullOrEmpty(form["Level"]) ? 0 : Convert.ToInt32(form["Level"]);
        return new CategoryCreateDTO
        {
            Thumbnail = thumbnail,
            Name = name!,
            Slug = slug!,
            IsActive = isActive,
            MainCategoryId = mainCategoryId,
            ParentCategoryId = parentCategoryId,
            Level = level
        };
    }
}
