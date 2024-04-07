using Microsoft.EntityFrameworkCore;
using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Persistence
{
  public class ChartPersistence : IChartPersistence
  {
    private readonly SeamstressContext _context;

    public ChartPersistence(SeamstressContext context)
    {
      this._context = context;
    }
    public async Task<List<Customer>> GetRegionCustomersAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      List<Customer> customers = await _context.Orders.Where(x =>
        DateOnly.FromDateTime(x.OrderedAt) >= periodBegin &&
        DateOnly.FromDateTime(x.OrderedAt) <= periodEnd
      )
      .Include(x => x.Customer)
      .Select(x => x.Customer!)
      .AsNoTracking().ToListAsync();

      return customers;
    }
    public async Task<List<ItemOrder>> GetModelItemOrdersAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      List<ItemOrder> itemOrders = await _context.ItemOrder.Where(x =>
        DateOnly.FromDateTime(x.Order!.OrderedAt) >= periodBegin &&
        DateOnly.FromDateTime(x.Order.OrderedAt) <= periodEnd
      )
      .Include(x => x.Item)
      .AsNoTracking().ToListAsync();

      return itemOrders;
    }

    public async Task<List<Order>> GetOrdersAsync(DateOnly periodBegin, DateOnly periodEnd)
    {
      List<Order> orders = await this._context.Orders.Where(x =>
        DateOnly.FromDateTime(x.OrderedAt) >= periodBegin &&
        DateOnly.FromDateTime(x.OrderedAt) <= periodEnd
      )
      .Include(x => x.SalePlatform)
      .OrderBy(x => x.OrderedAt)
      .AsNoTracking().ToListAsync();

      return orders;
    }
  }
}