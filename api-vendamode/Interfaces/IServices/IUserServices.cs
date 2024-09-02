using api_vendace.Entities.Users;
using api_vendace.Entities.Users.Security;
using api_vendace.Models;
using api_vendace.Models.Dtos.AuthDto;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos.AuthDto;
using api_vendamode.Models.Dtos.AuthDto.RoleDto;

namespace api_vendace.Interfaces.IServices;

public interface IUserServices
{
    Task<ServiceResponse<bool>> CreateUserAsync(UserCreateDTO userCreate);
    Task<ServiceResponse<Guid>> RegisterUserAsync(string mobileNumber, string passCode);
    Task<ServiceResponse<Guid>> EditUserProfileAsync(UserProfileUpdateDTO userProfileUpdate);
    Task<ServiceResponse<LoginDTO>> AuthenticateUserAsync(string mobileNumber, string plainPassword);
    Task<ServiceResponse<LoginDTO>> GetUserInfo(string mobileNumber, HttpContext context);
    Task<ServiceResponse<UserDTO>> GetUserBy(Guid userId);
    Task<ServiceResponse<List<User>>> GetAllUsersAsync();
    Guid GetUserId();
    Task<ServiceResponse<GenerateNewTokenResultDTO>> GenerateNewToken(GenerateNewTokenDTO generateNewToken);
    Task<ServiceResponse<Pagination<UserDTO>>> GetUsers(RequestQuery requestQuery);
    Task<ServiceResponse<UserDTO>> GetUserInfoMe(HttpContext context);

    Task<ServiceResponse<bool>> UpsertPermission(PermissionUpsertDTO permissionUpsert);
    Task<ServiceResponse<List<Permission>>> GetPermissions();

    Task<ServiceResponse<bool>> UpsertRole(RoleUpsertDTO roleUpsert);
    Task<ServiceResponse<List<Role>>> GetRoles();
}
