using Seamstress.Domain;

namespace Seamstress.Persistence.Contracts
{
  public interface IItemColorPersistence
  {
    Task<ItemColor[]> GetAllItemColorsByItemAsync(int itemId);
    Task<ItemColor> GetItemColorByIdAsync(int itemId, int colorId);
  }
}