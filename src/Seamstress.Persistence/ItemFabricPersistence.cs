using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class ItemFabricPersistence : IItemFabricPersistence
  {
    private readonly SeamstressContext _context;

    public ItemFabricPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<ItemFabric[]> GetAllItemFabricsByItemAsync(int itemId)
    {
      IQueryable<ItemFabric> query = _context.ItemsFabrics.Where(IF => IF.ItemId == itemId);

      return await query.AsNoTracking().ToArrayAsync();
    }

    public async Task<ItemFabric> GetItemFabricByIdAsync(int itemId, int fabricId)
    {
      IQueryable<ItemFabric> query = _context.ItemsFabrics.Where(ic => ic.FabricId == fabricId && ic.ItemId == itemId);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}