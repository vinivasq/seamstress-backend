using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class ItemColorPersistence : IItemColorPersistence
  {
    private readonly SeamstressContext _context;

    public ItemColorPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<ItemColor[]> GetAllItemColorsByItemAsync(int itemId)
    {
      IQueryable<ItemColor> query = _context.ItemsColors.Where(ic => ic.ItemId == itemId);

      return await query.AsNoTracking().ToArrayAsync();
    }

    public async Task<ItemColor> GetItemColorByIdAsync(int itemId, int colorId)
    {
      IQueryable<ItemColor> query = _context.ItemsColors.Where(ic => ic.ColorId == colorId && ic.ItemId == itemId);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}