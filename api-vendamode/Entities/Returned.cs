using api_vendace.Models;
using api_vendamode.Enums;

namespace api_vendamode.Entities;

public class Returned
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public CanceledType CanceledType { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;
}