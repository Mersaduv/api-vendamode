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
            Title = product.Title,
            Code = product.Code,
            ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl
            (product.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl!,
                Placeholder = img.Placeholder!
            }).ToList(), nameof(Product)),
            Info = product.Info?.Select(infoDto => new ProductAttributeDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
            Specifications = product.Specifications?.Select(infoDto => new ProductAttributeDto
            {
                Title = infoDto.Title,
                Value = infoDto.Value,
            }).ToList(),
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            Description = product.Description,
            Discount = product.Discount,
            InStock = product.InStock,
            Price = product.Price,
            Size = product.Size,
            Colors = product.Colors,
            Slug = product.Slug,
            NumReviews = product.Review?.Count,
            Sold = product.Sold,
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
                ImagesSrc = byteFileUtility.GetEncryptedFileActionUrl
                (prod.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl!,
                    Placeholder = img.Placeholder!
                }).ToList(), nameof(Product)),
                Code = prod.Code,
                Info = prod.Info?.Select(infoDto => new ProductAttributeDto
                {
                    Title = infoDto.Title,
                    Value = infoDto.Value,
                }).ToList(),
                Specifications = prod.Specifications?.Select(infoDto => new ProductAttributeDto
                {
                    Title = infoDto.Title,
                    Value = infoDto.Value,
                }).ToList(),
                CategoryId = prod.CategoryId,
                BrandId = prod.BrandId,
                Description = prod.Description,
                Discount = prod.Discount,
                InStock = prod.InStock,
                Price = prod.Price,
                Size = prod.Size,
                Colors = prod.Colors,
                Slug = prod.Slug,
                Sold = prod.Sold,
                NumReviews = prod.Review?.Count,
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