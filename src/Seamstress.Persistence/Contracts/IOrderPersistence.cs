using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IOrderPersistence
  {
    Task<Order[]> GetAllOrdersAsync();
    Task<Order> GetOrderByIdAsync(int orderId);
  }
}