using api_vendace.Const;
using api_vendace.Models;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.designDto;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class BannerEndpoints
{
    public static IEndpointRouteBuilder MapBannerApi(this IEndpointRouteBuilder apiGroup)
    {
        var bannerGroup = apiGroup.MapGroup(Constants.Banner);

        apiGroup.MapGet(Constants.Banners, GetAllBanners);
        apiGroup.MapGet(Constants.ArticleBanners, GetAllArticleBanners);
        apiGroup.MapGet($"footer-{Constants.Banners}", GetAllFooterBanners);

        bannerGroup.MapPost(string.Empty, CreateBanner)
        .Accepts<BannerCreateDto>("multipart/form-data");

        bannerGroup.MapPost("update", UpdateBanner)
        .Accepts<BannerUpsertDto>("multipart/form-data");

        bannerGroup.MapPost("upsert", UpsertBanners)
        .Accepts<BannerBulkUpsertDto>("multipart/form-data");
        bannerGroup.MapPost($"{Constants.Article}-upsert", UpsertArticleBanners)
        .Accepts<ArticleBannerBulkUpsertDto>("application/json");

        bannerGroup.MapPost("banner-footer/upsert", UpsertFooterBanner)
        .Accepts<FooterBannerUpsertDto>("multipart/form-data");

        bannerGroup.MapPut(string.Empty, UpdateBanner);

        bannerGroup.MapGet("{id:guid}", GetBanner);

        bannerGroup.MapDelete("{id:guid}", DeleteBanner);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateBanner(IBannerServices bannerServices,
               BannerCreateDto banner, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Banner");

        var result = await bannerServices.AddBanner(banner);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateBanner(IBannerServices bannerServices, BannerUpsertDto banner, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Banner");

        var result = await bannerServices.UpdateBanner(banner);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpsertBanners(IBannerServices bannerServices, BannerBulkUpsertDto request, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Upsert Banners");

        var result = await bannerServices.UpsertBanners(request.Banners);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<bool>>> UpsertArticleBanners(IBannerServices bannerServices, ArticleBannerBulkUpsertDto request, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Upsert Article Banners");

        var result = await bannerServices.UpsertArticleBanners(request.ArticleBanners);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpsertFooterBanner(IBannerServices bannerServices, FooterBannerUpsertDto banner, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Upsert Footer Banner");

        var result = await bannerServices.UpsertFooterBanner(banner);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteBanner(IBannerServices bannerServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Banner");

        var result = await bannerServices.DeleteBanner(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<BannerDto>>> GetBanner(IBannerServices bannerServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Banner");

        var result = await bannerServices.GetBannerBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<BannerDto>>>> GetAllBanners(IBannerServices bannerServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Banners");

        var result = await bannerServices.GetBanners();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<ArticleBannerDto>>>> GetAllArticleBanners(IBannerServices bannerServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Article Banners");

        var result = await bannerServices.GetArticleBanners();

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<IReadOnlyList<FooterBannerDto>>>> GetAllFooterBanners(IBannerServices bannerServices, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Getting all Footer Banners");

        var result = await bannerServices.GetFooterBanners();

        return TypedResults.Ok(result);
    }
}