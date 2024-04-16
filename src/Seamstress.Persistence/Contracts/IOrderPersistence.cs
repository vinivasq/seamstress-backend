using Seamstress.Domain;
using Seamstress.Persistence.Helpers;

namespace Seamstress.Persistence.Contracts
{
  public interface IOrderPersistence
  {
    Task<PageList<Order>> GetOrdersAsync(OrderParams orderParams);
    Task<Order[]> GetPendingOrdersAsync();
    Task<Order[]> GetPendingOrdersByExecutor(int userId);
    Task<Order?> GetOrderByIdAsync(int orderId);
  }
}