namespace api_vendamode.Models.Dtos;

public class CanceledDTO
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}