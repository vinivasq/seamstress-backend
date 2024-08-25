using Seamstress.Domain;
using Seamstress.Persistence.Context;
using Seamstress.Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using Seamstress.Persistence.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Seamstress.DTO;

namespace Seamstress.Persistence
{
  public class OrderPersistence : IOrderPersistence
  {
    private readonly SeamstressContext _context;
    private readonly IMapper _mapper;

    public OrderPersistence(SeamstressContext context, IMapper mapper)
    {
      this._context = context;
      this._mapper = mapper;
    }

    public async Task<PageList<OrderOutputDto>> GetOrdersAsync(OrderParams orderParams)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.SalePlatform);
      query = query.Include(order => order.Customer).ThenInclude(customer => customer!.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item)
        .ThenInclude(item => item!.SetItem).ThenInclude(setItem => setItem!.Set);

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

      IQueryable<OrderOutputDto> queryDto = query.ProjectTo<OrderOutputDto>(_mapper.ConfigurationProvider);

      return await PageList<OrderOutputDto>.CreateAsync(queryDto, orderParams.PageNumber, orderParams.PageSize);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.ItemOrders);
      query = query.Where(order => order.Id == orderId);

      return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<OrderOutputDto?> GetOrderOutputDtoByIdAsync(int orderId)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.SalePlatform);
      query = query.Include(order => order.Executor);
      query = query.Include(order => order.Customer).ThenInclude(customer => customer!.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item)
        .ThenInclude(item => item!.SetItem).ThenInclude(setItem => setItem!.Set);
      query = query.Where(order => order.Id == orderId);

      return await query.AsNoTracking().ProjectTo<OrderOutputDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<OrderOutputDto[]> GetPendingOrdersAsync()
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.Customer).ThenInclude(customer => customer!.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item)
        .ThenInclude(item => item!.SetItem).ThenInclude(setItem => setItem!.Set);
      query = query.Where(order => order.Step != Domain.Enum.Step.Entregue);
      query = query.OrderBy(order => order.Deadline);

      return await query.AsNoTracking().ProjectTo<OrderOutputDto>(_mapper.ConfigurationProvider).ToArrayAsync();
    }

    public async Task<OrderOutputDto[]> GetPendingOrdersByExecutor(int userId)
    {
      IQueryable<Order> query = _context.Orders;

      query = query.Include(order => order.Customer).ThenInclude(customer => customer!.Sizings);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Color);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Fabric);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Size);
      query = query.Include(order => order.ItemOrders).ThenInclude(itemOrder => itemOrder.Item)
        .ThenInclude(item => item!.SetItem).ThenInclude(setItem => setItem!.Set);
      query = query.Where(order => order.Step != Domain.Enum.Step.Entregue);
      query = query.Where(order => order.ExecutorId == userId);
      query = query.OrderBy(order => order.Deadline);

      return await query.AsNoTracking().ProjectTo<OrderOutputDto>(_mapper.ConfigurationProvider).ToArrayAsync();
    }
  }
}