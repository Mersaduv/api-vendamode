using api_vendamode.Const;
using api_vendamode.Entities.Users;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.AuthDto;
using api_vendamode.Models.Query;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api_vendamode.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthApi(this IEndpointRouteBuilder apiGroup)
    {
        var authGroup = apiGroup.MapGroup(Constants.Auth);
        apiGroup.MapGet(Constants.Users, GetUsers);
        authGroup.MapPost($"/{Constants.Register}/{{number}}/{{passCode}}", RegisterUser);
        authGroup.MapPost($"/{Constants.Login}/{{number}}/{{passCode}}", LogInUser);
        // authGroup.MapPost(Constants.ChangePassword, ChangePasswordAsync).RequireAuthorization();
        authGroup.MapPost(Constants.GenerateRefreshToken, GenerateNewToken);

        return apiGroup;
    }

    private static async Task<Results<Ok<ServiceResponse<GenerateNewTokenDTO>>, BadRequest<ServiceResponse<GenerateNewTokenDTO>>>> GenerateNewToken(
    IUserServices userServices, GenerateNewTokenDTO request)
    {
        var response = await userServices.GenerateNewToken(request);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ServiceResponse<Guid>>, BadRequest<ServiceResponse<Guid>>>> RegisterUser(
    IUserServices userServices, string number, string passCode)
    {
        var response = await userServices.RegisterUserAsync(number, passCode);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ServiceResponse<LoginDTO>>, BadRequest<ServiceResponse<LoginDTO>>>> LogInUser(
    IUserServices userServices, string number, string passCode)
    {
        var response = await userServices.AuthenticateUserAsync(number, passCode);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private async static Task<Ok<ServiceResponse<Pagination<UserDTO>>>> GetUsers(IUserServices userServices, [AsParameters] RequestQuery requestQuery, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Users");

        var result = await userServices.GetUsers(requestQuery);

        return TypedResults.Ok(result);
    }
}