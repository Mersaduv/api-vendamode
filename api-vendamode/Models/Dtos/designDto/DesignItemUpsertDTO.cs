using System.Reflection;
namespace api_vendamode.Models.Dtos.designDto;
public class DesignItemUpsertDTO
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public IFormFile? Thumbnail { get; set; }
    public string Link { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Index { get; set; }

    public static async ValueTask<DesignItemUpsertDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        if (!Guid.TryParse(form["Id"], out var id))
        {

        }
        var title = form["Title"];

        var thumbnail = form.Files.GetFile("Thumbnail");

        var link = form["Link"];
        var type = form["Type"];
        var indexStrForm = form["Index"];
        var indexStr = string.IsNullOrEmpty(indexStrForm) ? 0 : Convert.ToInt32(indexStrForm);

        if (!bool.TryParse(form["IsActive"], out var isActive))
        {
        }

        return new DesignItemUpsertDTO
        {
            Id = id,
            Title = title,
            Thumbnail = thumbnail, // Thumbnail can be null, indicating no change in the image
            Link = link!,
            Type = type!,
            Index = indexStr,
            IsActive = isActive
        };
    }
}
