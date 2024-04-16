using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using Seamstress.Persistence.Helpers;

namespace Seamstress.Persistence
{
  public class OrderPersistence : IOrderPersistence
  {
    private readonly SeamstressContext _context;
    public OrderPersistence(SeamstressContext context)
    {
      this._context = context;
    }

    public async Task<PageList<Order>> GetOrdersAsync(OrderParams orderParams)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.SalePlatform);
      query = query.Include(order => order.Customer).ThenInclude(customer => customer!.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item).ThenInclude(item => item!.Set);

      query = query.Where(order => order.OrderedAt.Date >= orderParams.OrderedAtStart.Date && order.OrderedAt.Date <= orderParams.OrderedAtEnd.Date);

      if (orderParams.CustomerId != null)
      {
        query = query.Where(order => order.CustomerId == orderParams.CustomerId);
      }

      if (orderParams.Steps?.Length > 0)
      {
        query = query.Where(order => orderParams.Steps.Contains((int)order.Step));
      }

      query = query.OrderBy(order => order.OrderedAt);

      return await PageList<Order>.CreateAsync(query, orderParams.PageNumber, orderParams.PageSize);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.SalePlatform);
      query = query.Include(order => order.Customer).ThenInclude(customer => customer!.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item).ThenInclude(item => item!.Set);
      query = query.Where(order => order.Id == orderId);

      return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<Order[]> GetPendingOrdersAsync()
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.Customer).ThenInclude(customer => customer!.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item).ThenInclude(item => item!.Set);
      query = query.Where(order => order.Step != Domain.Enum.Step.Entregue);
      query = query.OrderBy(order => order.Deadline);

      return await query.AsNoTracking().ToArrayAsync();
    }

    public async Task<Order[]> GetPendingOrdersByExecutor(int userId)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.Customer).ThenInclude(customer => customer!.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item).ThenInclude(item => item!.Set);
      query = query.Where(order => order.Step != Domain.Enum.Step.Entregue);
      query = query.OrderBy(order => order.Deadline);

      return await query.Where(order => order.ExecutorId == userId).AsNoTracking().ToArrayAsync();
    }
  }
}