namespace api_vendamode.Models.Dtos.designDto;

public class GeneralSettingUpsertDTO
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortIntroduction { get; set; } = string.Empty;
    public string GoogleTags { get; set; } = string.Empty;
}