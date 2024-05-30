using api_vendamode.Entities.Products;
using api_vendamode.Models.Dtos;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Utility;

namespace api_vendamode.Mapper;

public static class GetProductMapper
{
    public static ProductDTO ToProductResponse(this Product product, ByteFileUtility byteFileUtility)
    {
        return new ProductDTO
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Title = product.Title,
            Code = product.Code,
            Slug = product.Slug,
            ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl
            (product.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl!,
                Placeholder = img.Placeholder!
            }).ToList(), nameof(Product)),
            Price = product.Price,
            Discount = product.Discount,
            BrandId = product.BrandId,
            IsFake = product.IsFake,
            InStock = product.InStock,
            Created = product.Created,
            LastUpdated = product.LastUpdated
        };
    }
    public static GetAllResponse ToProductsResponse(this IEnumerable<Product> products, ByteFileUtility byteFileUtility)
    {
        return new GetAllResponse
        {
            Products = products.Select(prod => new ProductDTO
            {
                Id = prod.Id,
                Title = prod.Title,
                Slug = prod.Slug,
                Code = prod.Code,
                ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl
                (prod.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl!,
                    Placeholder = img.Placeholder!
                }).ToList(), nameof(Product)),
                CategoryId = prod.CategoryId,
                BrandId = prod.BrandId,
                Description = prod.Description,
                Discount = prod.Discount,
                InStock = prod.InStock,
                Price = prod.Price,
                Sold = prod.Sold,
                ReviewCount = prod.Review?.Count,
                Created = prod.Created,
                LastUpdated = prod.LastUpdated
            }).ToList()
        };
    }
}

public class GetAllResponse
{
    public IEnumerable<ProductDTO> Products { get; set; } = Enumerable.Empty<ProductDTO>();
}