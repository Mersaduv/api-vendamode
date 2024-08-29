using System.Reflection;

namespace api_vendamode.Models.Dtos.designDto;

public class BannerBulkUpsertDto
{
    public List<BannerUpsertDto> Banners { get; set; } = new List<BannerUpsertDto>();

    public static async ValueTask<BannerBulkUpsertDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();
        var bulkUpsertDto = new BannerBulkUpsertDto();

        var bannersCount = form.Keys
            .Where(key => key.StartsWith("Banners["))
            .Select(key => key.Split('[', ']')[1])
            .Distinct()
            .Count();

        for (int i = 0; i < bannersCount; i++)
        {
            var bannerDto = new BannerUpsertDto
            {
                Id = form.TryGetValue($"Banners[{i}].Id", out var idValue) && Guid.TryParse(idValue, out var id) ? id : (Guid?)null,
                CategoryId = form.TryGetValue($"Banners[{i}].CategoryId", out var categoryIdValue) && Guid.TryParse(categoryIdValue, out var categoryId) ? categoryId : (Guid?)null,
                Thumbnail = form.Files.GetFile($"Banners[{i}].Thumbnail"),
                Link = form.TryGetValue($"Banners[{i}].Link", out var linkValue) ? linkValue.ToString() : string.Empty,
                Type = form.TryGetValue($"Banners[{i}].Type", out var typeValue) ? typeValue.ToString() : string.Empty,
                IsActive = form.TryGetValue($"Banners[{i}].IsActive", out var isActiveValue) && bool.TryParse(isActiveValue, out var isActive) ? isActive : false,
                Index = form.TryGetValue($"Banners[{i}].Index", out var indexValue) && int.TryParse(indexValue, out var index) ? index : 0
            };

            bulkUpsertDto.Banners.Add(bannerDto);
        }

        return bulkUpsertDto;
    }
}
