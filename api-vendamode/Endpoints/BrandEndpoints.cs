using api_vendace.Const;
using api_vendace.Entities.Products;
using api_vendace.Filter;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Brand;
using api_vendace.Models.Query;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendace.Endpoints;

public static class BrandEndpoints
{
    public static IEndpointRouteBuilder MapBrandApi(this IEndpointRouteBuilder apiGroup)
    {
        var brandGroup = apiGroup.MapGroup(Constants.Brand);

        apiGroup.MapGet(Constants.AllBrands, GetAllBrands);
        apiGroup.MapGet(Constants.Brands, GetBrands);

        brandGroup.MapPost(string.Empty, CreateBrand)
        .Accepts<BrandCommandDTO>("multipart/form-data")
        .ProducesValidationProblem();

        brandGroup.MapPost("update", UpdateBrand)
        .Accepts<BrandCommandDTO>("multipart/form-data")
        .ProducesValidationProblem();

        brandGroup.MapGet("{id:guid}", GetBrand);

        brandGroup.MapDelete("{id:guid}", DeleteBrand);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateBrand(IBrandServices brandServices,
               BrandCommandDTO brand, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Brand");

        var result = await brandServices.AddBrand(brand);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateBrand(IBrandServices brandServices, BrandCommandDTO brand, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Brand");

        var result = await brandServices.UpdateBrand(brand);

        return TypedResults.Ok(result);
    }



    private async static Task<Ok<ServiceResponse<bool>>> DeleteBrand(IBrandServices brandServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Brand");

        var result = await brandServices.DeleteBrand(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<BrandDTO>>> GetBrand(IBrandServices brandServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Brand");

        var result = await brandServices.GetBrandBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<Brand>>>> GetAllBrands(IBrandServices brandServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Brands");

        var result = await brandServices.GetAllBrands();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Pagination<BrandDTO>>>> GetBrands(IBrandServices brandServices, [AsParameters] RequestQuery requestQuery, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Brands");

        var result = await brandServices.GetBrands(requestQuery);

        return TypedResults.Ok(result);
    }

}
