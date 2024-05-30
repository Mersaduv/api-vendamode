using api_vendamode.Const;
using api_vendamode.Entities.Products;
using api_vendamode.Filter;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Models.Dtos.ProductDto.Sizes;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class ProductSizeEndpoints
{
    public static IEndpointRouteBuilder MapProductSizeApi(this IEndpointRouteBuilder apiGroup)
    {
        var sizesGroup = apiGroup.MapGroup(Constants.Sizes);
        var sizeGroup = apiGroup.MapGroup(Constants.Size);

        sizeGroup.MapPost(string.Empty, CreateSize);

        sizeGroup.MapGet("{id:guid}", GetSize);

        sizeGroup.MapGet("category-sizes/{id:guid}", GetSizeByCategoryId);

        sizeGroup.MapDelete("/{id:guid}", DeleteSize)
        
        .AddEndpointFilter<GuidValidationFilter>()
        .ProducesValidationProblem();

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateSize(IProductSizeServices productSizeServices, SizeCreateDTO sizeCreate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create size");

        var result = await productSizeServices.AddSize(sizeCreate);

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
}

