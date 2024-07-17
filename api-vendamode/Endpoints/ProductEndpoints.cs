using api_vendace.Const;
using api_vendace.Entities.Products;
using api_vendace.Filter;
using api_vendace.Interfaces.IServices;
using api_vendace.Mapper;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Query;
using api_vendace.Utility;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Models.Query;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendace.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductApi(this IEndpointRouteBuilder apiGroup)
    {
        var productsGroup = apiGroup.MapGroup(Constants.Products);
        var productGroup = apiGroup.MapGroup(Constants.Product);

        productsGroup.MapGet(string.Empty, GetAllProduct);

        productsGroup.MapGet(Constants.Main, GetProductQuery);
        apiGroup.MapGet(Constants.ProductList, GetProductList);
        productsGroup.MapGet("/category/{id:guid}", GetProductByCategoryId);
        productGroup.MapGet("/{id:guid}", GetProduct);
        productGroup.MapGet(string.Empty, GetProductBySlug);

        apiGroup.MapGet($"/{Constants.ImageApi}/{{entity}}/{{fileName}}", MediaEndpoint);

        var adminProductGroup = productGroup.MapGroup(string.Empty); //.RequireAuthorization();

        adminProductGroup.MapPost(string.Empty, CreateProduct)
        .Accepts<ProductCreateDTO>("multipart/form-data")
        .ProducesValidationProblem();


        adminProductGroup.MapPost("update", UpdateProduct)
        .Accepts<ProductUpdateDTO>("multipart/form-data");

        adminProductGroup.MapPost("bulk-update", BulkUpdateProduct)
        .Accepts<BulkUpdateProductDTO>("application/json");

        adminProductGroup.MapDelete("{id:guid}", DeleteProduct);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> BulkUpdateProduct(BulkUpdateProductDTO dto, IProductServices productService)
    {
        var result = await productService.BulkUpdateProductStatus(dto.ProductIds, dto.IsActive);
        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteProduct(IProductServices productServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Product");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await productServices.DeleteAsync(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<GetProductsResult>>> GetProductQuery([AsParameters] RequestQuery parameters, IProductServices productService, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Products by Query");

        var result = await productService.GetProductsPagination(parameters);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<GetProductsResult>>> GetProductList([AsParameters] RequestQuery parameters, IProductServices productService, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Products by Query");

        var result = await productService.GetProducts(parameters);

        return TypedResults.Ok(result);
    }


    //Write
    private static async Task<Ok<ServiceResponse<bool>>> CreateProduct(IProductServices productServices,
                  ProductCreateDTO product_C_DTO, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Product");

        // await AccessControl.CheckProductPermissionFlag(context, "product-add");

        var productDTO = await productServices.CreateProduct(product_C_DTO);

        return TypedResults.Ok(productDTO);
    }

    // Read 
    private async static Task<Ok<ServiceResponse<ProductDTO>>> GetProduct(IProductServices productService, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Product");

        var result = await productService.GetSingleProductBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<ProductDTO>>> GetProductBySlug(IProductServices productService, ILogger<Program> _logger, [AsParameters] RequestBy request)
    {
        _logger.Log(LogLevel.Information, "Get Product");

        var result = await productService.GetBy(request.Slug!);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<List<ProductDTO>>>> GetProductByCategoryId(IProductServices productService, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Product");

        var result = await productService.GetByCategory(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IEnumerable<Product>>>> GetAllProduct(IProductServices productService, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Getting all Products");

        // await AccessControl.CheckProductPermissionFlag(context , "product-get-all");

        var result = await productService.GetAll();

        return TypedResults.Ok(result);
    }

    private async static Task<FileContentHttpResult> MediaEndpoint(string fileName, string entity, ByteFileUtility byteFileUtility, HttpContext context)
    {
        var filePath = byteFileUtility.GetFileFullPath(fileName, entity);
        byte[] encryptedData = await System.IO.File.ReadAllBytesAsync(filePath);

        //? Decrypt if the file is encrypted
        // var decryptedData = byteFileUtility.DecryptFile(encryptedData);

        context.Response.Headers.Append("Content-Disposition", "inline; filename=preview.jpg");

        return TypedResults.File(encryptedData, "image/jpeg");
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateProduct(IProductServices productService, ProductUpdateDTO productUpdate, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Product");

        var result = await productService.UpdateProduct(productUpdate);

        return TypedResults.Ok(result);
    }
}