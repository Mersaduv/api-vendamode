using api_vendace.Const;
using api_vendace.Filter;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Models.Dtos.ProductDto.Feature;
using api_vendamode.Models.Dtos.ProductDto.Category;
using api_vendamode.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using api_vendamode.Models.Query;
using static api_vendace.Services.Products.CategoryServices;

namespace api_vendace.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryApi(this IEndpointRouteBuilder apiGroup)
    {
        var categoryGroup = apiGroup.MapGroup(Constants.Category);

        apiGroup.MapGet(Constants.Categories, GetCategories);
        apiGroup.MapGet($"{Constants.Categories}/parents", GetCategories);
        apiGroup.MapGet(Constants.CategoriesTree, CategoriesTree).RequireAuthorization();

        categoryGroup.MapGet("{id:guid}", GetCategory);
        categoryGroup.MapGet("parentSub/{id:guid}", GetParentSubCategory);
        categoryGroup.MapGet("subCategories", GetSubCategory);

        categoryGroup.MapPost(string.Empty, CreateCategory)
        .Accepts<CategoryCreateDTO>("multipart/form-data");

        categoryGroup.MapPost("update", UpdateCategory)
        .Accepts<CategoryUpdateDTO>("multipart/form-data");

        categoryGroup.MapPost($"feature-update", CategoryFeatureUpdate);

        categoryGroup.MapDelete("{id:guid}", DeleteCategory);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateCategory(ICategoryServices categoryServices,
           CategoryCreateDTO category, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Category");

        var result = await categoryServices.AddCategory(category);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<List<CategoryDTO>>>> GetParentSubCategory(ICategoryServices categoryServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Getting parent SubCategories");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.GetParentSubCategoryAsync(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<SubCategoryResult>>> GetSubCategory(ICategoryServices categoryServices, ILogger<Program> _logger,  [AsParameters] RequestSubCategory requestSub)
    {
        _logger.Log(LogLevel.Information, "Getting SubCategories");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.GetSubCategoryAsync(requestSub);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> CategoryFeatureUpdate(ICategoryServices categoryServices, ILogger<Program> _logger, [AsParameters] CategoryFeatureUpdateDTO categoryFeatureUpdate)
    {
        _logger.Log(LogLevel.Information, "Getting parent SubCategories");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.UpdateFeatureInCategory(categoryFeatureUpdate);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IEnumerable<CategoryDTO>>>> CategoriesTree(ICategoryServices categoryServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Categories Tree");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.GetCategoriesTreeAsync();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<CategoryResult>>> GetCategories(ICategoryServices categoryServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Categories Tree");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.GetAllCategories();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteCategory(ICategoryServices categoryServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Category");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.Delete(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<CategoryDTO>>> GetCategory(ICategoryServices categoryServices,
  ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Category");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.GetBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateCategory(ICategoryServices categoryServices, CategoryUpdateDTO categoryUpdate, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Category");

        var result = await categoryServices.UpdateCategory(categoryUpdate);

        return TypedResults.Ok(result);
    }
}