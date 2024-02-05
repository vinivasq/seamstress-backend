using Seamstress.Application.Dtos;
using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface IItemOrderService
  {
    public ItemOrder[] UpdateItemOrders();

    public Task<ItemOrder> AddItemOrder(ItemOrderInputDto model);
    public Task<bool> DeleteItemOrder(int id);

    public Task<ItemOrderOutputDto[]> GetItemOrdersByOrderId(int orderId);
    public Task<ItemOrderOutputDto> GetItemOrderById(int id);
  }
}