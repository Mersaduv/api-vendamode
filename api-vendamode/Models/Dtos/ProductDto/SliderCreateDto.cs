using System.Reflection;
namespace api_vendamode.Models.Dtos.ProductDto;


public class SliderCreateDto
{
    public Guid? CategoryId { get; set; }
    public required IFormFile Thumbnail { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool IsMain { get; set; }

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

        var title = form["Title"];
        var uri = form["Uri"];

        if (!bool.TryParse(form["IsPublic"], out var isPublic))
        {
            // Handle error: IsPublic must be a boolean
            return null;
        }

        if (!bool.TryParse(form["IsMain"], out var isMain))
        {
            // Handle error: IsMain must be a boolean
            return null;
        }

        return new SliderCreateDto
        {
            CategoryId = categoryId,
            Thumbnail = url,
            Title = title!,
            Uri = uri!,
            IsPublic = isPublic,
            IsMain = isMain
        };
    }
}
