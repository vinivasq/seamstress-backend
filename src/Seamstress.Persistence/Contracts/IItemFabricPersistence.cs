using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IItemFabricPersistence
  {
    Task<ItemFabric[]> GetAllItemFabricsByItemAsync(int itemId);
    Task<ItemFabric> GetItemFabricByIdAsync(int itemId, int fabricId);
  }
}