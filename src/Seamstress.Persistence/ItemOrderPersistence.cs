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

    public ItemOrder[] UpdateItemOrders()
    {
      try
      {
        var itemOrders = _context.ItemOrder.AsNoTracking().ToArray();

        _context.AttachRange(itemOrders);

        foreach (var itemOrder in itemOrders)
        {
          var matchingItemSize = _context.ItemsSizes.FirstOrDefault(itemSize => itemSize.ItemId == itemOrder.ItemId && itemSize.SizeId == itemOrder.SizeId);

          if (matchingItemSize != null)
          {
            itemOrder.ItemSizeId = matchingItemSize.Id;
          }
        }

        _context.SaveChanges();

        return itemOrders;

      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
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