using System.Reflection;
namespace api_vendamode.Models.Dtos.designDto;
public class FooterBannerUpsertDto
{
    public Guid? Id { get; set; }
    public Guid? CategoryId { get; set; }
    public IFormFile? Thumbnail { get; set; } // Thumbnail can be optional in update
    public string Link { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public static async ValueTask<FooterBannerUpsertDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        if (!Guid.TryParse(form["Id"], out var id))
        {

        }
        var categoryIdStr = form["CategoryId"];
        var categoryId = string.IsNullOrEmpty(categoryIdStr) ? null : (Guid?)Guid.Parse(categoryIdStr);

        var thumbnail = form.Files.GetFile("Thumbnail");

        var link = form["Link"];
        var type = form["Type"];

        if (!bool.TryParse(form["IsActive"], out var isActive))
        {
            // Handle error: IsActive must be a boolean
        }

        return new FooterBannerUpsertDto
        {
            Id = id,
            CategoryId = categoryId,
            Thumbnail = thumbnail, // Thumbnail can be null, indicating no change in the image
            Link = link!,
            Type = type!,
            IsActive = isActive,
        };
    }
}