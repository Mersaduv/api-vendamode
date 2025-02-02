using api_vendace.Entities.Products;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Utility;

namespace api_vendace.Mapper;

public static class GetProductMapper
{
    public static ProductDTO ToProductResponse(this Product product, ByteFileUtility byteFileUtility)
    {
        return new ProductDTO
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Title = product.Title,
            StockTag = product.StockTag,
            Code = product.Code,
            Slug = product.Slug,
            Author = product.Author,
            MainImageSrc = byteFileUtility.GetEncryptedFileActionUrl
            ([new EntityImageDto
                            {
                                Id = product.MainImage!.Id,
                                ImageUrl = product.MainImage.ImageUrl!,
                                Placeholder = product.MainImage.Placeholder!
                            }],
            nameof(Product), product.Code).First(),
            ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl
            (product.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl!,
                Placeholder = img.Placeholder!
            }).ToList(), nameof(Product), product.Code),
            Price = product.Price,
            Discount = product.Discount,
            BrandId = product.BrandId,
            IsFake = product.IsFake,
            Status = product.Status,
            IsActive = product.IsActive,
            Date = product.Date,
            InStock = product.InStock,
            Sold = product.Sold,
            Description = product.Description,
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
                StockTag = prod.StockTag,
                Slug = prod.Slug,
                Code = prod.Code,
                Author = prod.Author,
                Date = prod.Date,
                ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl
                (prod.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl!,
                    Placeholder = img.Placeholder!
                }).ToList(), nameof(Product), prod.Code),
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