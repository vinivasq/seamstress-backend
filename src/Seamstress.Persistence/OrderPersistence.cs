using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Seamstress.Persistence
{
  public class OrderPersistence : IOrderPersistence
  {
    private readonly SeamstressContext _context;
    public OrderPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<Order[]> GetAllOrdersAsync()
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.Customer).ThenInclude(customer => customer.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.AditionalSizing);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item).ThenInclude(item => item.Set);
      query = query.OrderBy(order => order.Deadline);

      return await query.AsNoTracking().ToArrayAsync();
    }

    public async Task<Order[]> GetOrdersByExecutor(int userId)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.Customer).ThenInclude(customer => customer.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.AditionalSizing);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item).ThenInclude(item => item.Set);
      query = query.OrderBy(order => order.Deadline);

      return await query.Where(order => order.ExecutorId == userId).AsNoTracking().ToArrayAsync();
    }

    public async Task<Order> GetOrderByIdAsync(int orderId)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.Customer).ThenInclude(customer => customer.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.AditionalSizing);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item).ThenInclude(item => item.Set);
      query = query.Where(order => order.Id == orderId);

      return await query.AsNoTracking().FirstAsync();
    }
  }
}