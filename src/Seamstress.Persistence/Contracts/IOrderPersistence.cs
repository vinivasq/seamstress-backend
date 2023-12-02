using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IOrderPersistence
  {
    Task<Order[]> GetAllOrdersAsync();
    Task<Order[]> GetPendingOrdersAsync();
    Task<Order[]> GetOrdersByExecutor(int userId);
    Task<Order[]> GetPendingOrdersByExecutor(int userId);
    Task<Order> GetOrderByIdAsync(int orderId);
  }
}