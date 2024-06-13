using api_vendace.Entities.Users;
using api_vendace.Models;
using api_vendace.Models.Query;

namespace api_vendamode.Interfaces.IServices;

public interface IAddressServices
{
    Task<ServiceResponse<bool>> AddAddress(Address address);
    Task<ServiceResponse<bool>> UpsertAddress(Address address);
    Task<ServiceResponse<bool>> DeleteAddress(Guid id);
    Task<ServiceResponse<Address>> GetBy(Guid id);
    Task<ServiceResponse<Pagination<Address>>> GetAddresses(RequestQuery requestQuery);
    Task<ServiceResponse<IReadOnlyList<Address>>> GetAllAddresses();
}