using Seamstress.Domain;
using Seamstress.DTO;
using Seamstress.Persistence.Models;

namespace Seamstress.Persistence.Contracts
{
  public interface IOrderPersistence
  {
    Task<PageList<OrderOutputDto>> GetOrdersAsync(OrderParams orderParams);
    Task<OrderOutputDto[]> GetPendingOrdersAsync();
    Task<OrderOutputDto[]> GetPendingOrdersByExecutor(int userId);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<OrderOutputDto?> GetOrderOutputDtoByIdAsync(int orderId);
  }
}