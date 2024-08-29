using api_vendamode.Entities.Products;

namespace api_vendamode.Models.Dtos.designDto;

public class StoreCategoryBulkDTO
{
    public List<StoreCategory> StoreCategories { get; set; } = new List<StoreCategory>();
}