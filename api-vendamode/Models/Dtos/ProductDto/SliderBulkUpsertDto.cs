using System.Reflection;

namespace api_vendamode.Models.Dtos.ProductDto;

public class SliderBulkUpsertDto
{
    public List<SliderUpsertDto> Sliders { get; set; } = new List<SliderUpsertDto>();

    public static async ValueTask<SliderBulkUpsertDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();
        var bulkUpsertDto = new SliderBulkUpsertDto();

        var slidersCount = form.Keys
            .Where(key => key.StartsWith("Sliders["))
            .Select(key => key.Split('[', ']')[1]) // Extract the index part of the key
            .Distinct()
            .Count();

        for (int i = 0; i < slidersCount; i++)
        {
            var sliderDto = new SliderUpsertDto
            {
                Id = form.TryGetValue($"Sliders[{i}].Id", out var idValue) && Guid.TryParse(idValue, out var id) ? id : (Guid?)null,
                CategoryId = form.TryGetValue($"Sliders[{i}].CategoryId", out var categoryIdValue) && Guid.TryParse(categoryIdValue, out var categoryId) ? categoryId : (Guid?)null,
                Thumbnail = form.Files.GetFile($"Sliders[{i}].Thumbnail"),
                Link = form.TryGetValue($"Sliders[{i}].Link", out var linkValue) ? linkValue.ToString() : string.Empty,
                Type = form.TryGetValue($"Sliders[{i}].Type", out var typeValue) ? typeValue.ToString() : string.Empty,
                IsActive = form.TryGetValue($"Sliders[{i}].IsActive", out var isActiveValue) && bool.TryParse(isActiveValue, out var isActive) ? isActive : false
            };

            bulkUpsertDto.Sliders.Add(sliderDto);
        }

        return bulkUpsertDto;
    }
}
