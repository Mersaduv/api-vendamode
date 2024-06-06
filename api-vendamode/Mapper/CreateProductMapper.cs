using api_vendace.Entities;
using api_vendace.Entities.Products;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Utility;

namespace api_vendace.Mapper;

public static class CreateProductMapper
{
    public static Product ToProducts(this ProductCreateDTO product_C_DTO, ByteFileUtility byteFileUtility)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Title = product_C_DTO.Title,
            Slug = product_C_DTO.Title.ToLower(),
            IsActive = product_C_DTO.IsActive,
            Images = byteFileUtility.SaveFileInFolder<EntityImage<Guid, Product>>(product_C_DTO.Thumbnail!, nameof(Product), false),//!Boolean true is encrypted and Boolean false is not encrypted
            CategoryId = product_C_DTO.CategoryId,
            Description = product_C_DTO.Description,
            IsFake = product_C_DTO.IsFake,
            BrandId = product_C_DTO.BrandId,
            InStock = product_C_DTO.InStock,
            Price = product_C_DTO.Price,
            Discount = product_C_DTO.Discount,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
        };
    }
}