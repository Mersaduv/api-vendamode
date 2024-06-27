using api_vendamode.Enums;

namespace api_vendamode.Models.Dtos;

public class ReturnedDTO
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public CanceledType CanceledType { get; set; }
}