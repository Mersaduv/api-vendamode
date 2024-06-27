using System.Net;
using api_vendace.Const;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Review;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos.ProductDto.Review;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api_vendamode.Endpoints;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewApi(this IEndpointRouteBuilder apiGroup)
    {
        var reviewsGroup = apiGroup.MapGroup(Constants.Reviews);
        // reviewsGroup.MapGet(Constants.Reviews, GetReviews);

        reviewsGroup.MapPost(string.Empty, CreateReview)
        .Accepts<ReviewCreateDTO>("multipart/form-data");
        reviewsGroup.MapGet("/{id:guid}", GetProductReviews);
        reviewsGroup.MapDelete("/{id:guid}", DeleteReview);
        reviewsGroup.MapGet("single-review/{id:guid}", GetReview);

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
}
