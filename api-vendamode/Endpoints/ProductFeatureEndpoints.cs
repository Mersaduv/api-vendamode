using api_vendamode.Const;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Feature;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class ProductFeatureEndpoints
{
    public static IEndpointRouteBuilder MapProductFeatureApi(this IEndpointRouteBuilder apiGroup)
    {
        var featuresGroup = apiGroup.MapGroup(Constants.Features);
        var featureGroup = apiGroup.MapGroup(Constants.Feature);

        featuresGroup.MapGet(string.Empty, GetFeatures);

        featureGroup.MapPost(string.Empty, CreateFeature);

        featureGroup.MapPut(string.Empty, UpdateFeature);

        featureGroup.MapGet("feature/{id:int}", GetFeature);

        featureGroup.MapDelete("{id:int}", DeleteFeature);

        featureGroup.MapGet("{id:int}", GetFeaturesByCategory);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateFeature(IFeatureServices featureServices, ProductFeatureCreateDTO productFeatureCreate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Feature");

        var result = await featureServices.AddFeature(productFeatureCreate);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateFeature(IFeatureServices featureServices, ProductFeatureUpdateDTO feature, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Feature");

        var result = await featureServices.UpdateFeature(feature);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteFeature(IFeatureServices featureServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Feature");

        var result = await featureServices.DeleteFeature(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<ProductFeature>>> GetFeature(IFeatureServices featureServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Feature");

        var result = await featureServices.GetFeatureBy(id);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<ProductFeature>>> GetFeaturesByCategory(IFeatureServices featureServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Feature by category");

        var result = await featureServices.GetFeaturesByCategory(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<ProductFeature>>>> GetFeatures(IFeatureServices featureServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Features");

        var result = await featureServices.GetAllFeatures();

        return TypedResults.Ok(result);
    }
}
