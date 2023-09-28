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
      query = query.Include(item => item.Set);
      query = query.OrderBy(item => item.Id);

      return await query.AsNoTracking().ToArrayAsync();
    }
    public async Task<Item> GetItemByIdAsync(int id)
    {
      IQueryable<Item> query = _context.Items;

      query = query.Include(item => item.ItemColors).ThenInclude(IC => IC.Color);
      query = query.Include(item => item.ItemFabrics).ThenInclude(IF => IF.Fabric);
      query = query.Include(item => item.ItemSizes).ThenInclude(IS => IS.Size);
      query = query.Include(item => item.Set);
      query = query.Where(x => x.Id == id);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}