using api_vendace.Models;
using api_vendace.Models.Query;
using api_vendamode.Entities;
using api_vendamode.Models.Dtos;

namespace api_vendamode.Interfaces.IServices;

public interface ICanceledServices
{
    Task<ServiceResponse<bool>> AddCanceled(CanceledDTO canceled);
    Task<ServiceResponse<bool>> DeleteCanceled(Guid id);
    Task<ServiceResponse<Pagination<Canceled>>> GetCanceleds(RequestQuery requestQuery);
    Task<ServiceResponse<Canceled>> GetBy(Guid id);
    Task<ServiceResponse<bool>> UpsertCanceled(CanceledDTO canceled);
    Task<ServiceResponse<bool>> AddReturned(ReturnedDTO returned);
    Task<ServiceResponse<Pagination<Returned>>> GetReturned(RequestQuery requestQuery);
}