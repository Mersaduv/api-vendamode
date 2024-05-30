using api_vendamode.Const;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Category;
using ApiAryanakala.Interfaces.IServices;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryApi(this IEndpointRouteBuilder apiGroup)
    {
        var categoryGroup = apiGroup.MapGroup(Constants.Category);

        apiGroup.MapGet(Constants.Categories, GetAllCategory);

        categoryGroup.MapPost(string.Empty, CreateCategory)
        .Accepts<CategoryCreateDTO>("multipart/form-data");

        categoryGroup.MapDelete("{id:int}", DeleteCategory);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateCategory(ICategoryServices categoryServices,
           CategoryCreateDTO category, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Category");

        var result = await categoryServices.AddCategory(category);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IEnumerable<CategoryDTO>>>> GetAllCategory(ICategoryServices categoryServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Categories");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.GetAllCategories(null, 10);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteCategory(ICategoryServices categoryServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Category");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await categoryServices.Delete(id);

        return TypedResults.Ok(result);
    }
}