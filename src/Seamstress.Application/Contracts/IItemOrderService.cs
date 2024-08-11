using Seamstress.Domain;
using Seamstress.DTO;

namespace Seamstress.Application.Contracts
{
  public interface IItemOrderService
  {
    public Task<ItemOrder> AddItemOrder(ItemOrderInputDto model);
    public Task<ItemOrder> UpdateItemOrder(int id, ItemOrderInputDto model);
    public Task<bool> DeleteItemOrder(int id);

    public Task<ItemOrderOutputDto[]> GetItemOrdersByOrderId(int orderId);
    public Task<ItemOrderOutputDto> GetItemOrderById(int id);
  }
}