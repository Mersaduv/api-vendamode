
using api_vendace.Entities;
using api_vendace.Models;

namespace api_vendamode.Entities.Products;

public class Slider : BaseClass<Guid>
{
    public Guid? CategoryId { get; set; }
    public EntityImage<Guid, Slider>? Image { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Uri { get; set; }
    public bool IsPublic { get; set; }
    public bool IsMain { get; set; }
}