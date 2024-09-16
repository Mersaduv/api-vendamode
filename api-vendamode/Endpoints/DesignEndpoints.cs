using api_vendace.Const;
using api_vendace.Entities.Users;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Brand;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Models.Query;
using api_vendamode.Entities.Designs;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.ArticleDto;
using api_vendamode.Models.Dtos.designDto;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Models.Query;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class DesignEndpoints
{
    public static IEndpointRouteBuilder MapDesignApi(this IEndpointRouteBuilder apiGroup)
    {
        var designGroup = apiGroup.MapGroup(Constants.Design);

        var articlesGroup = apiGroup.MapGroup(Constants.Articles);
        var articleGroup = apiGroup.MapGroup(Constants.Article);

        articleGroup.MapPost(string.Empty, UpsertArticle).Accepts<ArticleUpsertDTO>("multipart/form-data");
        articleGroup.MapGet("{id:guid}", GetArticle);
        articlesGroup.MapGet(string.Empty, GetAllArticles);
        articleGroup.MapDelete("{id:guid}", DeleteArticle);
        articleGroup.MapPost("trash/{id:guid}", DeleteTrashArticle);
        articleGroup.MapPost("restore/{id:guid}", RestoreArticle);
        articleGroup.MapGet(string.Empty, GetArticleBySlug);
        articleGroup.MapGet("category", GetArticleByCategory);

        // designItem 
        designGroup.MapPost("items", UpsertDesignItem).Accepts<DesignBulkUpsertDto>("multipart/form-data");
        designGroup.MapDelete("items/{id:guid}", DeleteDesignItems);
        designGroup.MapGet("items", GetDesignItems);
        //

        designGroup.MapPost("storeCategory", UpsertStoreCategories).Accepts<StoreCategoryBulkDTO>("application/json");
        designGroup.MapDelete("storeCategory/{id:guid}", DeleteStoreCategory);
        designGroup.MapGet("storeCategories", GetStoreCategories);
        designGroup.MapGet("storeCategoryList", GetStoreCategoryList);

        designGroup.MapPost("storeBrand", UpsertStoreBrands).Accepts<StoreBrandBulkDTO>("application/json");
        designGroup.MapDelete("storeBrand/{id:guid}", DeleteStoreBrand);
        designGroup.MapGet("storeBrands", GetStoreBrands);
        designGroup.MapGet("storeBrandList", GetStoreBrandList);

        designGroup.MapPost("logoImages", UpsertLogoImages).Accepts<LogoUpsertDTO>("multipart/form-data");
        designGroup.MapGet("logoImages", GetLogoImages);

        designGroup.MapPost("headerText", UpsertHeaderText);
        designGroup.MapGet("headerText", GetHeaderText);

        designGroup.MapPost("support", UpsertSupport);
        designGroup.MapGet("support", GetSupport);

        designGroup.MapPost("redirects", UpsertRedirect);
        designGroup.MapGet("redirects", GetRedirect);

        designGroup.MapPost("generalSetting", GeneralSetting);
        designGroup.MapGet("generalSetting", GetGeneralSetting);

        designGroup.MapPost("sloganFooter", UpsertSloganFooter);
        designGroup.MapGet("sloganFooter", GetSloganFooter);

        designGroup.MapPost("copyright", UpsertCopyright);
        designGroup.MapGet("copyright", GetCopyright);


        designGroup.MapPost("columnFooters", UpsertColumnFooters);
        designGroup.MapGet("columnFooters", GetColumnFooters);
        designGroup.MapDelete("columnFooters/{id:guid}", DeleteColumnFooter);
        designGroup.MapDelete("footerArticleColumn/{id:guid}", DeleteFooterArticleColumn);

        return apiGroup;
    }
    private async static Task<Ok<ServiceResponse<IReadOnlyList<BrandDTO>>>> GetStoreBrandList(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Store Brand List");

        var result = await designServices.GetStoreBrandList();

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<bool>>> DeleteStoreBrand(IDesignServices designServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Store Brand");

        var result = await designServices.DeleteStoreBrand(id);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<IReadOnlyList<StoreBrand>>>> GetStoreBrands(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Store Brands");

        var result = await designServices.GetStoreBrands();

        return TypedResults.Ok(result);
    }
    private static async Task<Ok<ServiceResponse<bool>>> UpsertStoreBrands(IDesignServices designServices,
                        StoreBrandBulkDTO storeBrandBulkDTO, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Upsert Store Brands");

        var result = await designServices.UpsertStoreBrands(storeBrandBulkDTO.StoreBrands);

        return TypedResults.Ok(result);
    }


    //...
    private async static Task<Ok<ServiceResponse<bool>>> DeleteFooterArticleColumn(IDesignServices designServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Column Footer");

        var result = await designServices.DeleteFooterArticleColumn(id);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<bool>>> DeleteColumnFooter(IDesignServices designServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Column Footer");

        var result = await designServices.DeleteColumnFooter(id);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<IReadOnlyList<CategoryDTO>>>> GetStoreCategoryList(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Store Category List");

        var result = await designServices.GetStoreCategoryList();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<List<ColumnFooter>>>> GetColumnFooters(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Column Footers");

        var result = await designServices.GetColumnFooters();

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpsertColumnFooters(IDesignServices designServices,
                                                                ColumnFooterBulkUpsertDTO columnFooterBulk, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Upsert Column Footers");

        var result = await designServices.UpsertColumnFooters(columnFooterBulk.ColumnFooters);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<Copyright>>> GetCopyright(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Copyright");

        var result = await designServices.GetCopyright();

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpsertCopyright(IDesignServices designServices,
                                                                Copyright copyright, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Upsert Copyright");

        var result = await designServices.UpsertCopyright(copyright);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<ArticleDto>>> GetArticleByCategory(IArticleServices articleServices, ILogger<Program> _logger, [AsParameters] RequestBy request)
    {
        _logger.Log(LogLevel.Information, "Get Article");

        var result = await articleServices.GetBy(request.Id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<ArticleDto>>> GetArticleBySlug(IArticleServices articleServices, ILogger<Program> _logger, [AsParameters] RequestBy request)
    {
        _logger.Log(LogLevel.Information, "Get Article");

        var result = await articleServices.GetBy(request.Slug);

        return TypedResults.Ok(result);
    }
    private static async Task<Ok<ServiceResponse<bool>>> UpsertRedirect(IDesignServices designServices,
                                                                    Redirects redirects, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Upsert Redirect");

        var result = await designServices.UpsertRedirect(redirects);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Redirects>>> GetRedirect(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Redirect");

        var result = await designServices.GetRedirect();

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpsertSupport(IDesignServices designServices,
                                                                        Support support, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Update Support");

        var result = await designServices.UpsertSupport(support);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Support>>> GetSupport(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Support");

        var result = await designServices.GetSupport();

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpsertSloganFooter(IDesignServices designServices,
       SloganFooter sloganFooter, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Update Slogan Footer");

        var result = await designServices.UpsertSloganFooter(sloganFooter);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<SloganFooter>>> GetSloganFooter(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Slogan Footer");

        var result = await designServices.GetSloganFooters();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteStoreCategory(IDesignServices designServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Store Category");

        var result = await designServices.DeleteStoreCategory(id);

        return TypedResults.Ok(result);
    }
    private async static Task<Ok<ServiceResponse<IReadOnlyList<StoreCategory>>>> GetStoreCategories(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Store Categories");

        var result = await designServices.GetStoreCategories();

        return TypedResults.Ok(result);
    }
    private static async Task<Ok<ServiceResponse<bool>>> UpsertStoreCategories(IDesignServices designServices,
                        StoreCategoryBulkDTO storeCategoryBulk, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Upsert Store Category");

        var result = await designServices.UpsertStoreCategory(storeCategoryBulk.StoreCategories);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<DesignItemDTO>>>> GetDesignItems(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Design Items");

        var result = await designServices.GetDesignItems();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteDesignItems(IDesignServices designServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Design Items");

        var result = await designServices.DeleteDesignItem(id);

        return TypedResults.Ok(result);
    }
    private static async Task<Ok<ServiceResponse<bool>>> UpsertDesignItem(IDesignServices designServices,
    DesignBulkUpsertDto designItemUpsert, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Upsert Design Items");

        var result = await designServices.UpsertDesignItems(designItemUpsert.DesignItems);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteTrashArticle(IArticleServices articleServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete to Trash Article");

        // await AccessControl.CheckProductPermissionFlag(context , "Article-get-all");

        var result = await articleServices.DeleteTrashAsync(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> RestoreArticle(IArticleServices articleServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Restore Product");

        // await AccessControl.CheckProductPermissionFlag(context , "Article-get-all");

        var result = await articleServices.RestoreArticleAsync(id);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<Pagination<ArticleDto>>>> GetAllArticles(IArticleServices articleServices, ILogger<Program> _logger, [AsParameters] RequestQuery requestQuery)
    {
        _logger.Log(LogLevel.Information, "Getting all Articles");

        var result = await articleServices.GetAllArticles(requestQuery);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<ArticleDto>>> GetArticle(IArticleServices articleServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Getting Article");

        var result = await articleServices.GetArticle(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteArticle(IArticleServices articleServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Article");

        var result = await articleServices.DeleteArticle(id);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<Guid>>> UpsertArticle(IArticleServices articleServices,
       ArticleUpsertDTO articleUpsert, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Upsert Article");

        var result = await articleServices.UpsertArticle(articleUpsert);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpsertHeaderText(IDesignServices designServices,
           HeaderTextUpsertDTO headerTextUpsert, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Update Header Text");

        var result = await designServices.UpsertHeaderText(headerTextUpsert);

        return TypedResults.Ok(result);
    }
    private static async Task<Ok<ServiceResponse<bool>>> GeneralSetting(IDesignServices designServices,
       GeneralSettingUpsertDTO generalSettingUpsert, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Update General Settings");

        var result = await designServices.UpsertGeneralSettings(generalSettingUpsert);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<HeaderText>>> GetHeaderText(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Header Text");

        var result = await designServices.GetHeaderText();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<GeneralSetting>>> GetGeneralSetting(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get General Setting");

        var result = await designServices.GetGeneralSettings();

        return TypedResults.Ok(result);
    }


    private static async Task<Ok<ServiceResponse<bool>>> UpsertLogoImages(IDesignServices designServices,
       LogoUpsertDTO logoUpsert, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Update Logo Images");

        var result = await designServices.UpsertLogoImages(logoUpsert);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<LogoImagesDTO>>>> GetLogoImages(IDesignServices designServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Logo Images");

        var result = await designServices.GetLogoImages();

        return TypedResults.Ok(result);
    }
}