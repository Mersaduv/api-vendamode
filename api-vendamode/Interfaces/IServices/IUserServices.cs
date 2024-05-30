using api_vendamode.Entities.Users;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.AuthDto;
using api_vendamode.Models.Query;

namespace api_vendamode.Interfaces.IServices;

public interface IUserServices
{
    Task<ServiceResponse<Guid>> RegisterUserAsync(string mobileNumber, string passCode);
    Task<ServiceResponse<LoginDTO>> AuthenticateUserAsync(string mobileNumber, string plainPassword);
    Task<ServiceResponse<User>> GetUserByIdAsync(Guid userId);
    Task<ServiceResponse<List<User>>> GetAllUsersAsync();
    Guid GetUserId();
    Task<ServiceResponse<GenerateNewTokenDTO>> GenerateNewToken(GenerateNewTokenDTO generateNewToken);
    Task<ServiceResponse<Pagination<UserDTO>>> GetUsers(RequestQuery requestQuery);
}
