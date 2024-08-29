using System.Reflection;
namespace api_vendamode.Models.Dtos.ProductDto;


public class SliderCreateDto
{
    public required IFormFile Thumbnail { get; set; }
    public Guid? CategoryId { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public static async ValueTask<SliderCreateDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var categoryId = string.IsNullOrEmpty(form["CategoryId"]) ? null : (Guid?)Guid.Parse(form["CategoryId"]!);

        var url = form.Files.GetFile("Thumbnail");
        if (url == null)
        {
            // Handle error: Url is required
            return null;
        }

        var link = form["Link"];
        var type = form["Type"];

        if (!bool.TryParse(form["IsActive"], out var isActive))
        {
            // Handle error: IsActive must be a boolean
            return null;
        }

        return new SliderCreateDto
        {
            CategoryId = categoryId,
            Thumbnail = url,
            Link = link!,
            Type = type!,
            IsActive = isActive,
        };
    }
}
