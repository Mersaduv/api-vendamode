using api_vendamode.Entities.Designs;
using api_vendamode.Entities.Products;

namespace api_vendamode.Models.Dtos.designDto;

public class StoreBrandBulkDTO
{
    public List<StoreBrand> StoreBrands { get; set; } = new List<StoreBrand>();
}