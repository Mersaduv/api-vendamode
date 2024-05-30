using System.Reflection;

namespace api_vendamode.Models.Dtos.ProductDto.Category;

public class CategoryCreateDTO
{
    public string Name { get; set; } = string.Empty;
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
        var isActive = bool.Parse(form["IsActive"]!);
        var mainCategoryId = string.IsNullOrEmpty(form["MainCategoryId"]) ? null : (Guid?)Guid.Parse(form["MainCategoryId"]!);
        var parentCategoryId = string.IsNullOrEmpty(form["ParentCategoryId"]) ? null : (Guid?)Guid.Parse(form["ParentCategoryId"]!);
        var level = int.Parse(form["Level"]!);

        return new CategoryCreateDTO
        {
            Thumbnail = thumbnail,
            Name = name!,
            IsActive = isActive,
            MainCategoryId = mainCategoryId,
            ParentCategoryId = parentCategoryId,
            Level = level
        };
    }
}
