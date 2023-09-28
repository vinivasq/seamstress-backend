using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IItemPersistence
  {
    Task<Item[]> GetAllItemsAsync();
    Task<Item> GetItemByIdAsync(int id);
  }
}