using System.Reflection;

namespace api_vendamode.Models.Dtos.ProductDto;
public class Thumbnails
{
    public List<IFormFile>? Thumbnail { get; set; }
    // public string? FileName { get; set; }
    public static async ValueTask<Thumbnails?> BindAsync(HttpContext context,
                                                                ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnailFiles = form.Files.GetFiles("Thumbnail");
        var thumbnail = thumbnailFiles.Any() ? thumbnailFiles.ToList() : null;
        // var fileName = form.TryGetValue("FileName", out var fileNameValue) ? fileNameValue.ToString() : null;

        return new Thumbnails
        {
            Thumbnail = thumbnail,
            // FileName = fileName
        };
    }


}