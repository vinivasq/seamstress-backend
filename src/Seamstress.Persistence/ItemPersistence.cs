using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Seamstress.Persistence
{
  public class ItemPersistence : IItemPersistence
  {
    private readonly SeamstressContext _context;

    public ItemPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<Item[]> GetAllItemsAsync()
    {
      IQueryable<Item> query = _context.Items;

      query = query.Include(item => item.ItemColors).ThenInclude(IC => IC.Color);
      query = query.Include(item => item.ItemFabrics).ThenInclude(IF => IF.Fabric);
      query = query.Include(item => item.ItemSizes).ThenInclude(IS => IS.Size);
      query = query.Include(item => item.ItemSizes).ThenInclude(IS => IS.Measurements);
      query = query.Include(item => item.Set);
      query = query.OrderBy(item => item.Id);

      return await query.AsNoTracking().ToArrayAsync();
    }
    public async Task<Item?> GetItemByIdAsync(int id)
    {
      IQueryable<Item> query = _context.Items;

      query = query.Include(item => item.ItemColors).ThenInclude(IC => IC.Color);
      query = query.Include(item => item.ItemFabrics).ThenInclude(IF => IF.Fabric);
      query = query.Include(item => item.ItemSizes).ThenInclude(IS => IS.Size);
      query = query.Include(item => item.ItemSizes).ThenInclude(IS => IS.Measurements);
      query = query.Include(item => item.Set);
      query = query.Where(x => x.Id == id);

      return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<Item?> GetItemWithoutAttributesAsync(int id)
    {
      IQueryable<Item> query = _context.Items;

      query = query.Include(item => item.ItemColors);
      query = query.Include(item => item.ItemFabrics);
      query = query.Include(item => item.ItemSizes).ThenInclude(IS => IS.Measurements);
      query = query.Where(x => x.Id == id);

      return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<bool> CheckFKAsync(int id)
    {
      return await _context.ItemOrder.AnyAsync(x => x.ItemId == id);
    }

    public async Task<Item[]> GetItemsByExternalSourceAsync(int salePlatformId)
    {
      IQueryable<Item> query = _context.Items
          .AsNoTracking()
          .Include(i => i.ItemColors).ThenInclude(ic => ic.Color)
          .Include(i => i.ItemFabrics).ThenInclude(ifab => ifab.Fabric)
          .Include(i => i.ItemSizes).ThenInclude(isz => isz.Size)
          .Where(i => i.SalePlatformId == salePlatformId)
          .OrderBy(i => i.Id);

      return await query.ToArrayAsync();
    }

    public async Task<Item?> GetItemByIdTrackedAsync(int id)
    {
      // Same as GetItemByIdAsync but WITHOUT AsNoTracking — for update operations
      IQueryable<Item> query = _context.Items
          .Include(i => i.ItemColors).ThenInclude(ic => ic.Color)
          .Include(i => i.ItemFabrics).ThenInclude(ifab => ifab.Fabric)
          .Include(i => i.ItemSizes).ThenInclude(isz => isz.Size)
          .Include(i => i.Set);

      return await query.FirstOrDefaultAsync(i => i.Id == id);
    }
  }
}