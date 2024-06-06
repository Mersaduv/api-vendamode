namespace api_vendace.Models.Dtos;

public class ObjectValue
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Value { get; set; }
}