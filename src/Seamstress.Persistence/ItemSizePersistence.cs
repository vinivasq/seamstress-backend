using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class ItemSizePersistence : IItemSizePersistence
  {
    private readonly SeamstressContext _context;
    public ItemSizePersistence(SeamstressContext context)
    {
      this._context = context;

    }
    public async Task<ItemSize[]> GetItemSizesByItemIdAsync(int id)
    {
      IQueryable<ItemSize> query = _context.ItemsSizes;

      query = query.Include(itemSize => itemSize.Item);
      query = query.Include(itemSize => itemSize.Size);
      query = query.Include(itemSize => itemSize.Measurements);
      query = query.Where(itemSize => itemSize.ItemId == id);

      return await query.AsNoTracking().ToArrayAsync();
    }
    public async Task<ItemSize> GetItemSizeByIdAsync(int id)
    {
      IQueryable<ItemSize> query = _context.ItemsSizes;

      query = query.Include(itemSize => itemSize.Item);
      query = query.Include(itemSize => itemSize.Size);
      query = query.Include(itemSize => itemSize.Measurements);
      query = query.Where(itemSize => itemSize.Id == id);

      return await query.AsNoTracking().FirstOrDefaultAsync() ?? throw new Exception("Tamanho de modelo não encontrado");
    }
    public async Task<ItemSize> GetOnlyItemSizeByIdAsync(int id)
    {
      IQueryable<ItemSize> query = _context.ItemsSizes;

      query = query.Where(itemSize => itemSize.Id == id);

      return await query.AsNoTracking().FirstOrDefaultAsync() ?? throw new Exception("Tamanho de modelo não encontrado");
    }
  }
}