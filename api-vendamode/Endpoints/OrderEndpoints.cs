
using api_vendace.Const;
using api_vendace.Models;
using api_vendace.Models.Query;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.ProductDto.Order;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderApi(this IEndpointRouteBuilder apiGroup)
    {
        var orderGroup = apiGroup.MapGroup(Constants.Order);

        orderGroup.MapPost(string.Empty, CreateOrder)
        .Accepts<OrderCreateDTO>("multipart/form-data");

        orderGroup.MapPost("place/{id}", PlaceOrder);

        orderGroup.MapGet(string.Empty, GetOrders);

        orderGroup.MapPost("update", UpdateOrder)
        .Accepts<OrderUpsertDTO>("application/json");

        orderGroup.MapPost("update-canceled", UpdateOrderCanceled)
        .Accepts<CancelOrderUpdateStatus>("application/json");
        
        orderGroup.MapPost("update-returned", UpdateOrderReturned)
        .Accepts<OrderReturnedDTO>("multipart/form-data");

        orderGroup.MapDelete("{id:guid}", DeleteOrder);

        return apiGroup;
    }

    private static async Task<Ok<ServiceResponse<Guid>>> CreateOrder(
     IOrderServices orderService, OrderCreateDTO orderCreate)
    {

        var response = await orderService.CreateOrder(orderCreate);
        return TypedResults.Ok(response);
    }


    private static async Task<Ok<ServiceResponse<Guid>>> PlaceOrder(
    IOrderServices orderService, Guid id)
    {
        var response = await orderService.PlaceOrder(id);
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<ServiceResponse<OrderResult>>> GetOrders(
        IOrderServices orderService, [AsParameters] RequestQuery query)
    {
        var response = await orderService.GetOrders(query);
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpdateOrder(
    IOrderServices orderService, OrderUpsertDTO orderUpdate)
    {
        var response = await orderService.UpdateOrder(orderUpdate);
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpdateOrderCanceled(
    IOrderServices orderService, CancelOrderUpdateStatus orderUpdate)
    {
        var response = await orderService.UpdateOrderStatus(orderUpdate);
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<ServiceResponse<bool>>> UpdateOrderReturned(
    IOrderServices orderService, OrderReturnedDTO orderUpdate)
    {
        var response = await orderService.UpdateOrderReturnedStatus(orderUpdate);
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<ServiceResponse<bool>>> DeleteOrder(
    IOrderServices orderService, Guid id)
    {
        var response = await orderService.DeleteOrder(id);
        return TypedResults.Ok(response);
    }
}