using Microsoft.EntityFrameworkCore;

namespace api_vendace.Models.Dtos;
[Owned]
public class EntityImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
}