using Seamstress.Application.Dtos;
using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface IItemSizeService
  {
    public Task<ItemSizeForMeasurementsDto[]> GetItemSizesByItemIdAsync(int id);
    public Task<ItemSizeForMeasurementsDto> GetItemSizeByIdAsync(int id);
    public ItemSizeDto GetItemSizeByItemOrder(int itemId, int sizeId);
    public Task<ItemSizeForMeasurementsDto> UpdateItemSizeAsync(int id, ItemSize model);
    public Task<bool> DeleteItemSizeAsync(int id);
  }
}