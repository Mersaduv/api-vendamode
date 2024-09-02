using System.Reflection;

namespace api_vendace.Models.Dtos.ProductDto.Category;

public class CategoryCreateDTO
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsActiveProduct { get; set; }
    public List<IFormFile>? Thumbnail { get; set; }
    public Guid? MainCategoryId { get; set; }
    public Guid? MainId { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int Level { get; set; }

    public static async ValueTask<CategoryCreateDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        var name = form["Name"];
        
        var isActive = bool.TryParse(form["IsActive"], out var isActiveResult) && isActiveResult;

        var isActiveProduct = bool.TryParse(form["IsActiveProduct"], out var isActiveProductResult) && isActiveProductResult;

        var mainCategoryId = string.IsNullOrEmpty(form["MainCategoryId"]) ? null : (Guid?)Guid.Parse(form["MainCategoryId"]!);
        var mainId = string.IsNullOrEmpty(form["MainId"]) ? null : (Guid?)Guid.Parse(form["MainId"]!);
        var parentCategoryId = string.IsNullOrEmpty(form["ParentCategoryId"]) ? null : (Guid?)Guid.Parse(form["ParentCategoryId"]!);
        var level = string.IsNullOrEmpty(form["Level"]) ? 0 : Convert.ToInt32(form["Level"]);

        return new CategoryCreateDTO
        {
            Thumbnail = thumbnail,
            Name = name!,
            IsActive = isActive,
            IsActiveProduct = isActiveProduct,
            MainCategoryId = mainCategoryId,
            MainId = mainId,
            ParentCategoryId = parentCategoryId,
            Level = level
        };
    }
}
