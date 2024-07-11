using api_vendace.Const;
using api_vendace.Entities.Products;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Feature;
using api_vendamode.Models.Dtos.ProductDto;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendace.Endpoints;

public static class ProductFeatureEndpoints
{
    public static IEndpointRouteBuilder MapProductFeatureApi(this IEndpointRouteBuilder apiGroup)
    {
        var featuresGroup = apiGroup.MapGroup(Constants.Features);
        var featureGroup = apiGroup.MapGroup(Constants.Feature);

        featuresGroup.MapGet(string.Empty, GetFeatures);
        featureGroup.MapGet("values", GetFeatureValues);

        featureGroup.MapPost(string.Empty, CreateFeature);
        featureGroup.MapPost("value", CreateFeatureValue);

        featureGroup.MapPut(string.Empty, UpdateFeature);
        featureGroup.MapPut("value", UpdateFeatureValue);

        featureGroup.MapGet("{id:guid}", GetFeature);
        featureGroup.MapGet("value/{id:guid}", GetFeatureValue);

        featureGroup.MapDelete("{id:guid}", DeleteFeature);
        featureGroup.MapDelete("value/{id:guid}", DeleteFeatureValue);

        featureGroup.MapGet("by-category/{id:guid}", GetFeaturesByCategory);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateFeature(IFeatureServices featureServices, ProductFeatureCreateDTO productFeatureCreate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Feature");

        var result = await featureServices.AddFeature(productFeatureCreate);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateFeatureValue(IFeatureServices featureServices, FeatureValueCreateDTO featureValueCreate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create FeatureValue");

        var result = await featureServices.AddFeatureValue(featureValueCreate);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateFeature(IFeatureServices featureServices, ProductFeatureUpdateDTO feature, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Feature");

        var result = await featureServices.UpdateFeature(feature);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateFeatureValue(IFeatureServices featureServices, ProductFeatureUpdateDTO featureValueUpdate, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update FeatureValue");

        var result = await featureServices.UpdateFeatureValue(featureValueUpdate);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteFeature(IFeatureServices featureServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Feature");

        var result = await featureServices.DeleteFeature(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteFeatureValue(IFeatureServices featureServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Feature");

        var result = await featureServices.DeleteFeatureValue(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<ProductFeature>>> GetFeature(IFeatureServices featureServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Feature");

        var result = await featureServices.GetFeatureBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<FeatureValue>>> GetFeatureValue(IFeatureServices featureServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get FeatureValue");

        var result = await featureServices.GetFeatureValueBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<GetCategoryFeaturesByCategory>>> GetFeaturesByCategory(IFeatureServices featureServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Feature by category");

        var result = await featureServices.GetFeaturesByCategory(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<List<ProductFeature>>>> GetFeatures(IFeatureServices featureServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Features");

        var result = await featureServices.GetAllFeatures();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<FeatureValue>>>> GetFeatureValues(IFeatureServices featureServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all FeatureValues");

        var result = await featureServices.GetAllFeatureValues();

        return TypedResults.Ok(result);
    }
}
