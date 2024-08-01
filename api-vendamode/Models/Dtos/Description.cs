using System.Reflection;
using api_vendace.Models;

namespace api_vendamode.Models.Dtos;

public class Description
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IFormFile? Thumbnail { get; set; }
        public static async ValueTask<Description?> BindAsync(HttpContext context,
                                                                ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        var thumbnail = form.Files.GetFile("Thumbnail");
         var name = form["Name"];
        // var fileName = form.TryGetValue("FileName", out var fileNameValue) ? fileNameValue.ToString() : null;

        return new Description
        {
            Thumbnail = thumbnail,
            Name = name!
        };
    }
}