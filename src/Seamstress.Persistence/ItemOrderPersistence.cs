using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class ItemOrderPersistence : IItemOrderPersistence
  {
    private readonly SeamstressContext _context;
    public ItemOrderPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public Task<ItemOrder[]> GetItemOrdersByOrderIdAsync(int orderId)
    {
      IQueryable<ItemOrder> query = _context.ItemOrder.Where(io => io.OrderId == orderId)
                                    .Include(io => io.Color)
                                    .Include(io => io.Fabric)
                                    .Include(io => io.ItemSize)
                                    .Include(io => io.Item);

      return query.AsNoTracking().ToArrayAsync();
    }

    public Task<ItemOrder> GetItemOrderByIdAsync(int id)
    {
      IQueryable<ItemOrder> query = _context.ItemOrder.Where(io => io.Id == id)
                                    .Include(io => io.Color)
                                    .Include(io => io.Fabric)
                                    .Include(io => io.ItemSize)
                                    .Include(io => io.Item);

      return query.AsNoTracking().FirstAsync();
    }
  }
}