using System.Reflection;

namespace api_vendamode.Models.Dtos.designDto;
public class DesignBulkUpsertDto
{
    public List<DesignItemUpsertDTO> DesignItems { get; set; } = new List<DesignItemUpsertDTO>();

    public static async ValueTask<DesignBulkUpsertDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var form = await context.Request.ReadFormAsync();
        var bulkUpsertDto = new DesignBulkUpsertDto();

        var designItemsCount = form.Keys
            .Where(key => key.StartsWith("DesignItems["))
            .Select(key => key.Split('[', ']')[1]) // Extract the index part of the key
            .Distinct()
            .Count();

        for (int i = 0; i < designItemsCount; i++)
        {
            var sliderDto = new DesignItemUpsertDTO
            {
                Id = form.TryGetValue($"DesignItems[{i}].Id", out var idValue) && Guid.TryParse(idValue, out var id) ? id : (Guid?)null,
                Title = form.TryGetValue($"DesignItems[{i}].Title", out var titleValue) ? titleValue.ToString() : string.Empty,
                Thumbnail = form.Files.GetFile($"DesignItems[{i}].Thumbnail"),
                Link = form.TryGetValue($"DesignItems[{i}].Link", out var linkValue) ? linkValue.ToString() : string.Empty,
                Type = form.TryGetValue($"DesignItems[{i}].Type", out var typeValue) ? typeValue.ToString() : string.Empty,
                IsActive = form.TryGetValue($"DesignItems[{i}].IsActive", out var isActiveValue) && bool.TryParse(isActiveValue, out var isActive) ? isActive : false,
                Index = form.TryGetValue($"DesignItems[{i}].Index", out var indexValue) && int.TryParse(indexValue, out var index) ? index : 0


            };

            bulkUpsertDto.DesignItems.Add(sliderDto);
        }

        return bulkUpsertDto;
    }
}
