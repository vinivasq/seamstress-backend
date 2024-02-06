using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface IItemSizeService
  {
    public Task<ItemSize[]> GetItemSizesByItemIdAsync(int id);
    public Task<ItemSize> GetItemSizeByIdAsync(int id);
    public ItemSize GetItemSizeByItemOrder(int itemId, int sizeId);
    public Task<ItemSize> UpdateItemSizeAsync(int id, ItemSize model);
    public Task<bool> DeleteItemSizeAsync(int id);
  }
}