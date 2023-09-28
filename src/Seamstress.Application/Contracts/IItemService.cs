using Seamstress.Application.Dtos;
using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface IItemService
  {
    public Task<ItemOutputDto> AddItem(ItemInputDto model);
    public Task<ItemOutputDto> UpdateITem(int id, ItemInputDto model);
    public Task<bool> DeleteITem(int id);

    public Task<Item[]> GetItemsAsync();
    public Task<Item> GetItemByIdAsync(int id);


  }
}