using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface IItemSizeService
  {
    public Task<ItemSize[]> GetItemSizesByItemIdAsync(int id);
    public Task<ItemSize> GetItemSizeByIdAsync(int id);
    public Task<ItemSize> UpdateItemSizeAsync(int id, ItemSize model);
    public Task<bool> DeleteItemSizeAsync(int id);
  }
}