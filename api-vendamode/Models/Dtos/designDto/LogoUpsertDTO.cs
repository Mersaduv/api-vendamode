using System.Reflection;

namespace api_vendamode.Models.Dtos.designDto;

public class LogoUpsertDTO
{
    public Guid? Id { get; set; }
    public IFormFile? OrgThumbnail { get; set; }
    public IFormFile? FaviconThumbnail { get; set; }
    public static async ValueTask<LogoUpsertDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var id = string.IsNullOrEmpty(form["Id"]) ? null : (Guid?)Guid.Parse(form["Id"]!);

        var orgThumbnail = form.Files.GetFile("OrgThumbnail");
        var faviconThumbnail = form.Files.GetFile("FaviconThumbnail");

        return new LogoUpsertDTO
        {
            Id = id,
            OrgThumbnail = orgThumbnail,
            FaviconThumbnail = faviconThumbnail
        };
    }
}