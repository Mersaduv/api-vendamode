using api_vendace.Const;
using api_vendace.Entities.Products;
using api_vendace.Filter;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Models.Query;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendace.Endpoints;

public static class ProductSizeEndpoints
{
    public static IEndpointRouteBuilder MapProductSizeApi(this IEndpointRouteBuilder apiGroup)
    {
        // /api/sizes
        var sizesGroup = apiGroup.MapGroup(Constants.Sizes);
        // /api/size
        var sizeGroup = apiGroup.MapGroup(Constants.Size);

        sizesGroup.MapGet(string.Empty, GetSizes);

        sizeGroup.MapPost(string.Empty, CreateSize);

        sizeGroup.MapPut(string.Empty, UpdateSize);

        sizeGroup.MapPost("category", CreateCategorySize)
        .Accepts<ProductSizeCreateDTO>("multipart/form-data");

        sizeGroup.MapPost("category-update", UpdateCategorySize)
        .Accepts<ProductSizeUpdateDTO>("multipart/form-data");

        sizeGroup.MapGet("{id:guid}", GetSize);
        sizeGroup.MapGet("product/{id:guid}", GetSize);

        sizeGroup.MapGet("category-sizes/{id:guid}", GetSizeByCategoryId);

        sizeGroup.MapGet("category-productSizes/{id:guid}", GetSizeByProductSizeId);

        sizeGroup.MapDelete("/{id:guid}", DeleteSize);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<Pagination<SizeDTO>>>> GetSizes(IProductSizeServices productSizeServices, [AsParameters] RequestQuery parameters, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Getting All Sizes");

        var result = await productSizeServices.GetAllSizes(parameters);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateSize(IProductSizeServices productSizeServices, SizeCreateDTO sizeCreate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create size");

        var result = await productSizeServices.AddSize(sizeCreate);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpdateSize(IProductSizeServices productSizeServices, SizeUpdateDTO sizeUpdate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Update size");

        var result = await productSizeServices.UpdateSize(sizeUpdate);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateCategorySize(IProductSizeServices productSizeServices, ProductSizeCreateDTO productSizeCreate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create ProductSize");

        var result = await productSizeServices.AddProductSize(productSizeCreate);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpdateCategorySize(IProductSizeServices productSizeServices, ProductSizeUpdateDTO productSizeUpdate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Update ProductSize");

        var result = await productSizeServices.UpdateProductSize(productSizeUpdate);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteSize(IProductSizeServices productSizeServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete size");

        var result = await productSizeServices.DeleteSize(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Sizes>>> GetSize(IProductSizeServices productSizeServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get size");

        var result = await productSizeServices.GetSizeBy(id);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<ProductSizeDTO>>> GetSizeByCategoryId(IProductSizeServices productSizeServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Product size");

        var result = await productSizeServices.GetProductSizeByCategory(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<ProductSizeDTO>>> GetSizeByProductSizeId(IProductSizeServices productSizeServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Product size");

        var result = await productSizeServices.GetCategoryByProductSize(id);

        return TypedResults.Ok(result);
    }
}

