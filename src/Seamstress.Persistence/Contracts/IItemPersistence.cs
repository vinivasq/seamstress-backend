using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IItemPersistence
  {
    Task<Item[]> GetAllItemsAsync();
    Task<Item?> GetItemByIdAsync(int id);
    Task<Item?> GetItemWithoutAttributesAsync(int id);
    Task<bool> CheckFKAsync(int id);
  }
}