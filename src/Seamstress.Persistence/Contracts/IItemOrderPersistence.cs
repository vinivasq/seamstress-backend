using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IItemOrderPersistence
  {
    ItemOrder[] UpdateItemOrders();
    Task<ItemOrder[]> GetItemOrdersByOrderIdAsync(int orderId);
    Task<ItemOrder> GetItemOrderByIdAsync(int id);
  }
}