using api_vendace.Const;
using api_vendace.Entities.Users;
using api_vendace.Models;
using api_vendace.Models.Query;
using api_vendamode.Interfaces.IServices;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressApi(this IEndpointRouteBuilder apiGroup)
    {
        var addressGroup = apiGroup.MapGroup(Constants.Address);

        apiGroup.MapGet(Constants.Addresses, GetAddresses);

        addressGroup.MapPost(string.Empty, CreateAddress).RequireAuthorization()
        .Accepts<Address>("application/json")
        .ProducesValidationProblem();

        addressGroup.MapPut(string.Empty, UpdateAddress)
        .Accepts<Address>("application/json");

        addressGroup.MapGet("{id:guid}", GetAddress);

        addressGroup.MapDelete("{id:guid}", DeleteAddress);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<bool>>> CreateAddress(IAddressServices addressServices,
           Address address, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create Address");

        var result = await addressServices.AddAddress(address);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<bool>>> UpdateAddress(IAddressServices addressServices, Address address, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Update Address");

        var result = await addressServices.UpsertAddress(address);

        return TypedResults.Ok(result);
    }



    private async static Task<Ok<ServiceResponse<bool>>> DeleteAddress(IAddressServices addressServices, Guid id, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Delete Address");

        var result = await addressServices.DeleteAddress(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Address>>> GetAddress(IAddressServices addressServices, ILogger<Program> _logger, Guid id)
    {
        _logger.Log(LogLevel.Information, "Get Address");

        var result = await addressServices.GetBy(id);

        return TypedResults.Ok(result);
    }

    private async static Task<Ok<ServiceResponse<Pagination<Address>>>> GetAddresses(IAddressServices addressServices, [AsParameters] RequestQuery requestQuery, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Addresses");

        var result = await addressServices.GetAddresses(requestQuery);

        return TypedResults.Ok(result);
    }
}