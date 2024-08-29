using System.Reflection;

namespace api_vendamode.Models.Dtos.ArticleDto;

public class ArticleUpsertDTO
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IFormFile? Thumbnail { get; set; }
    public int Place { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public static async ValueTask<ArticleUpsertDTO?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();

        if (!Guid.TryParse(form["Id"], out var id))
        {

        }
        var categoryIdStr = form["CategoryId"];
        var categoryId =  string.IsNullOrEmpty(categoryIdStr) ? null : (Guid?)Guid.Parse(categoryIdStr!);
        var titleIdStr = form["Title"];

        var thumbnail = form.Files.GetFile("Thumbnail");

        var description = form["Description"];
        var placeStrForm = form["Place"];
        var placeStr = string.IsNullOrEmpty(placeStrForm) ? 0 : Convert.ToInt32(placeStrForm);

        if (!bool.TryParse(form["IsActive"], out var isActive))
        {
            // Handle error: IsActive must be a boolean
        }

        return new ArticleUpsertDTO
        {
            Id = id,
            Title = titleIdStr!,
            Thumbnail = thumbnail, // Thumbnail can be null, indicating no change in the image
            IsActive = isActive,
            Description = description!,
            Place = placeStr,
            CategoryId = categoryId
        };
    }
}