using api_vendace.Const;
using api_vendace.Models;
using api_vendace.Models.Query;
using api_vendamode.Entities;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class CanceledEndpoints
{
    public static IEndpointRouteBuilder MapCanceledApi(this IEndpointRouteBuilder apiGroup)
    {
        var canceledGroup = apiGroup.MapGroup(Constants.Canceleds);
        var returnedGroup = apiGroup.MapGroup(Constants.Returned);

        canceledGroup.MapGet(string.Empty, GetCanceleds);
        returnedGroup.MapGet(string.Empty, GetReturneds);

        canceledGroup.MapPost(string.Empty, CreateCanceled)
            .Accepts<CanceledDTO>("application/json");
        returnedGroup.MapPost(string.Empty, CreateReturned)
        .Accepts<ReturnedDTO>("application/json");

        canceledGroup.MapPut(string.Empty, UpdateCanceled)
            .Accepts<CanceledDTO>("application/json");

        canceledGroup.MapGet("{id:guid}", GetCanceled);

        canceledGroup.MapDelete("{id:guid}", DeleteCanceled);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateCanceled(ICanceledServices canceledServices,
        CanceledDTO canceled, ILogger<Program> _logger, HttpContext context)
    {
        _logger.LogInformation("Create Canceled");
        var result = await canceledServices.AddCanceled(canceled);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateReturned(ICanceledServices canceledServices,
    ReturnedDTO returned, ILogger<Program> _logger, HttpContext context)
    {
        _logger.LogInformation("Create Returned");
        var result = await canceledServices.AddReturned(returned);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpdateCanceled(ICanceledServices canceledServices,
        CanceledDTO canceled, ILogger<Program> _logger)
    {
        _logger.LogInformation("Update Canceled");
        var result = await canceledServices.UpsertCanceled(canceled);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> DeleteCanceled(ICanceledServices canceledServices,
        Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.LogInformation("Delete Canceled");
        var result = await canceledServices.DeleteCanceled(id);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<Canceled>>> GetCanceled(ICanceledServices canceledServices,
        ILogger<Program> _logger, Guid id)
    {
        _logger.LogInformation("Get Canceled");
        var result = await canceledServices.GetBy(id);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<Pagination<Canceled>>>> GetCanceleds(ICanceledServices canceledServices,
        [AsParameters] RequestQuery requestQuery, ILogger<Program> _logger)
    {
        _logger.LogInformation("Get Canceleds");
        var result = await canceledServices.GetCanceleds(requestQuery);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<Pagination<Returned>>>> GetReturneds(ICanceledServices canceledServices,
        [AsParameters] RequestQuery requestQuery, ILogger<Program> _logger)
    {
        _logger.LogInformation("Get Canceleds");
        var result = await canceledServices.GetReturned(requestQuery);
        return TypedResults.Ok(result);
    }
}
