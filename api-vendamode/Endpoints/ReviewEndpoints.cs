using System.Net;
using api_vendace.Const;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Review;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos.ProductDto.Review;
using api_vendamode.Models.Dtos.ProductDto.Review.Article;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api_vendamode.Endpoints;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewApi(this IEndpointRouteBuilder apiGroup)
    {
        var reviewsGroup = apiGroup.MapGroup(Constants.Reviews);
        // reviewsGroup.MapGet(Constants.Reviews, GetReviews);

        reviewsGroup.MapPost(string.Empty, CreateReview).Accepts<ReviewCreateDTO>("multipart/form-data");
        reviewsGroup.MapGet("/{id:guid}", GetProductReviews);
        reviewsGroup.MapDelete("/{id:guid}", DeleteReview);
        reviewsGroup.MapGet("single-review/{id:guid}", GetReview);

        var articleReviewsGroup = apiGroup.MapGroup(Constants.ArticleReviews);

        apiGroup.MapGet("all-articleReviews", GetAllArticleReviews);
        articleReviewsGroup.MapPost(string.Empty, CreateArticleReview);
        articleReviewsGroup.MapGet("/{id:guid}", GetArticleReviews);
        articleReviewsGroup.MapDelete("/{id:guid}", DeleteArticleReview);
        articleReviewsGroup.MapGet("single/{id:guid}", GetArticleReview);

        return apiGroup;
    }

    private async static Task<Ok<ServiceResponse<ReviewDto>>> GetReview(IReviewServices reviewServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Get Single Review");

        var result = await reviewServices.GetReviewBy(id);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<bool>>> DeleteReview(IReviewServices reviewServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Review");

        var result = await reviewServices.DeleteReview(id);

        return TypedResults.Ok(result);
    }


    // private async static Task<Ok<ServiceResponse<PagingModel<ReviewDto>>>> GetReviews(IReviewServices reviewServices, [FromQuery] int page, ILogger<Program> _logger, HttpContext context)
    // {
    //     _logger.Log(LogLevel.Information, "Get Reviews");

    //     var result = await reviewServices.GetReviews(page);

    //     return TypedResults.Ok(result);
    // }


    private async static Task<Ok<ServiceResponse<bool>>> CreateReview(IReviewServices reviewServices, ReviewCreateDTO command, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Review");

        var result = await reviewServices.CreateReview(command);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<ReviewResult>>> GetProductReviews(IReviewServices reviewServices, [AsParameters] RequestQuery query, Guid id, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Product Reviews");

        var result = await reviewServices.GetProductReviews(id, query);

        return TypedResults.Ok(result);
    }


    private async static Task<Ok<ServiceResponse<bool>>> CreateArticleReview(IReviewServices articleReviewServices, ArticleReviewCreateDTO command, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Article Review");

        var result = await articleReviewServices.CreateArticleReview(command);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<ArticleReviewDto>>> GetArticleReview(IReviewServices articleReviewServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Get Single Article Review");

        var result = await articleReviewServices.GetArticleReviewBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> DeleteArticleReview(IReviewServices articleReviewServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Article Review");

        var result = await articleReviewServices.DeleteArticleReview(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Pagination<ArticleReviewDto>>>> GetArticleReviews(IReviewServices articleReviewServices, [AsParameters] RequestQuery query, Guid id, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Article Reviews");

        var result = await articleReviewServices.GetArticleReviews(id, query);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Pagination<ArticleReviewDto>>>> GetAllArticleReviews(IReviewServices articleReviewServices, [AsParameters] RequestQuery query, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get All Article Reviews");

        var result = await articleReviewServices.GetAllArticleReviews( query);

        return TypedResults.Ok(result);
    }
}
