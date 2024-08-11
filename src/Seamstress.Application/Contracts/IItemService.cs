using Seamstress.Domain;
using Seamstress.DTO;

namespace Seamstress.Application.Contracts
{
  public interface IItemService
  {
    public Task<ItemOutputDto> AddItem(ItemInputDto model);
    public Task<ItemOutputDto> UpdateITem(int id, ItemInputDto model);
    public Task<ItemOutputDto> SetActiveState(int id, bool state);
    public Task<bool> CheckFK(int id);
    public Task<bool> DeleteITem(int id);

    public Task<Item[]> GetItemsAsync();
    public Task<Item> GetItemByIdAsync(int id);

  }
}