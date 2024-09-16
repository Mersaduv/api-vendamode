using api_vendace.Models;

namespace api_vendamode.Entities.Designs;

public class StoreBrand : BaseClass<Guid>
{
    public Guid BrandId { get; set; }
    public bool IsActive { get; set; }
}