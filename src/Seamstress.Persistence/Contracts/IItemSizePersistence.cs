using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IItemSizePersistence
  {

    Task<ItemSize[]> GetItemSizesByItemIdAsync(int id);
    Task<ItemSize> GetItemSizeByIdAsync(int id);
    Task<ItemSize> GetOnlyItemSizeByIdAsync(int id);
    ItemSize GetItemSizeByItemOrder(int itemId, int sizeId);
  }
}