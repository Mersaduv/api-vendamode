using api_vendace.Const;
using api_vendace.Entities.Users;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos.AuthDto;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos.AuthDto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api_vendace.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthApi(this IEndpointRouteBuilder apiGroup)
    {
        var authGroup = apiGroup.MapGroup(Constants.Auth);
        apiGroup.MapGet(Constants.Users, GetUsers);
        apiGroup.MapGet($"/{Constants.User}/info", GetUserInfo).RequireAuthorization();
        apiGroup.MapGet($"/{Constants.User}/info/me", GetUserInfoMe).RequireAuthorization();
        apiGroup.MapPut($"/{Constants.User}", EditUserProfile).Accepts<UserProfileUpdateDTO>("application/json").RequireAuthorization();

        authGroup.MapPost($"/{Constants.Register}", RegisterUser).Accepts<UserQueryDTO>("application/json");
        authGroup.MapPost($"/{Constants.Login}", LogInUser).Accepts<UserQueryDTO>("application/json");
        // authGroup.MapPost(Constants.ChangePassword, ChangePasswordAsync).RequireAuthorization();
        authGroup.MapPost(Constants.GenerateRefreshToken, GenerateNewToken);
        authGroup.MapPost(Constants.AddUser, AddUser).Accepts<UserCreateDTO>("multipart/form-data");

        return apiGroup;
    }

    private static async Task<Results<Ok<ServiceResponse<GenerateNewTokenResultDTO>>, BadRequest<ServiceResponse<GenerateNewTokenResultDTO>>>> GenerateNewToken(
    IUserServices userServices, GenerateNewTokenDTO request)
    {
        var response = await userServices.GenerateNewToken(request);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ServiceResponse<Guid>>, BadRequest<ServiceResponse<Guid>>>> EditUserProfile(
    IUserServices userServices, UserProfileUpdateDTO request)
    {
        var response = await userServices.EditUserProfileAsync(request);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }


    private static async Task<Results<Ok<ServiceResponse<Guid>>, BadRequest<ServiceResponse<Guid>>>> RegisterUser(
    IUserServices userServices, UserQueryDTO userQuery)
    {
        var response = await userServices.RegisterUserAsync(userQuery.MobileNumber, userQuery.Password);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ServiceResponse<LoginDTO>>, BadRequest<ServiceResponse<LoginDTO>>>> GetUserInfo(
    IUserServices userServices, [FromQuery] string mobileNumber, HttpContext context)
    {
        var response = await userServices.GetUserInfo(mobileNumber, context);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ServiceResponse<UserDTO>>, BadRequest<ServiceResponse<UserDTO>>>> GetUserInfoMe(
    IUserServices userServices, HttpContext context)
    {
        var response = await userServices.GetUserInfoMe(context);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ServiceResponse<LoginDTO>>, BadRequest<ServiceResponse<LoginDTO>>>> LogInUser(
    IUserServices userServices, UserQueryDTO userQuery)
    {
        var response = await userServices.AuthenticateUserAsync(userQuery.MobileNumber, userQuery.Password);
        return !response.Success ? TypedResults.BadRequest(response) : TypedResults.Ok(response);
    }

    private async static Task<Ok<ServiceResponse<Pagination<UserDTO>>>> GetUsers(IUserServices userServices, [AsParameters] RequestQuery requestQuery, ILogger<Program> _logger)
    {
        _logger.Log(LogLevel.Information, "Get Users");

        var result = await userServices.GetUsers(requestQuery);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ServiceResponse<bool>>> AddUser(IUserServices userServices,
              UserCreateDTO userCreate, ILogger<Program> _logger, HttpContext context)
    {
        _logger.Log(LogLevel.Information, "Create User");

        // await AccessControl.CheckProductPermissionFlag(context, "product-add");

        var result = await userServices.CreateUserAsync(userCreate);

        return TypedResults.Ok(result);
    }
}