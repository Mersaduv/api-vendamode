using api_vendace.Models;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos.ProductDto.Order;

namespace api_vendamode.Interfaces.IServices;

public interface IOrderServices
{
    Task<ServiceResponse<Guid>> CreateOrder(OrderCreateDTO orderCreated);
    Task<ServiceResponse<bool>> UpdateOrder(OrderUpsertDTO orderUpdate);
    Task<ServiceResponse<bool>> UpdateOrderStatus(CancelOrderUpdateStatus orderUpdate);
    Task<ServiceResponse<bool>> UpdateOrderReturnedStatus(OrderReturnedDTO orderUpdate);
    Task<ServiceResponse<OrderResult>> GetOrders(RequestQuery query);
    Task<ServiceResponse<bool>> DeleteOrder(Guid orderId);
    Task<ServiceResponse<Guid>> PlaceOrder(Guid id);

}