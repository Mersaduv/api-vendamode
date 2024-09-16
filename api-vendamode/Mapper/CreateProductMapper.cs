using api_vendace.Entities;
using api_vendace.Entities.Products;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Utility;

namespace api_vendace.Mapper;

public static class CreateProductMapper
{
    public static Product ToProducts(this ProductCreateDTO product_C_DTO, ByteFileUtility byteFileUtility, Guid productId)
    {
        return new Product
        {
            Id = productId,
            Title = product_C_DTO.Title,
            Slug = product_C_DTO.Title.ToLower(),
            Status = product_C_DTO.Status,
            IsActive = product_C_DTO.IsActive,
            Date = product_C_DTO.Date,
            MainImage = byteFileUtility.SaveFileInFolder<EntityMainImage<Guid, Product>>([product_C_DTO.MainThumbnail], nameof(Product), false).First(),
            Images = byteFileUtility.SaveFileInFolder<EntityImage<Guid, Product>>(product_C_DTO.Thumbnail != null ? product_C_DTO.Thumbnail : [], nameof(Product), false),//!Boolean true is encrypted and Boolean false is not encrypted
            CategoryId = product_C_DTO.CategoryId,
            Description = product_C_DTO.Description,
            IsFake = product_C_DTO.IsFake,
            BrandId = product_C_DTO.BrandId,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
        };
    }
}