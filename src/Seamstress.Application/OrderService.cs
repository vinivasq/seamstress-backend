using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.Domain.Enum;
using Seamstress.DTO;
using Seamstress.Persistence.Contracts;
using Seamstress.Persistence.Models;

namespace Seamstress.Application
{
  public class OrderService : IOrderService
  {
    private readonly IGeneralPersistence _generalPersistence;
    private readonly IOrderPersistence _orderPersistence;
    private readonly IItemPersistence _itemPersistence;
    private readonly IUserPersistence _userPersistence;
    private readonly IMapper _mapper;

    public OrderService(IGeneralPersistence generalPersistence,
                        IOrderPersistence orderPersistence,
                        IItemPersistence itemPersistence,
                        IUserPersistence userPersistence,
                        IMapper mapper
                        )
    {
      this._orderPersistence = orderPersistence;
      this._itemPersistence = itemPersistence;
      this._userPersistence = userPersistence;
      this._generalPersistence = generalPersistence;
      this._mapper = mapper;
    }

    public async Task<OrderOutputDto> AddOrder(OrderInputDto model)
    {
      try
      {
        await ValidateItemOrder(model);

        var order = _mapper.Map<Order>(model);

        _generalPersistence.Add<Order>(order);

        if (await _generalPersistence.SaveChangesAsync())
        {
          Order orderResponse = await _orderPersistence.GetOrderByIdAsync(order.Id)
            ?? throw new Exception("Não foi possível encontrar o pedido após cadastro");

          return _mapper.Map<OrderOutputDto>(orderResponse);
        }

        throw new Exception("Não foi possível criar o Evento");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
    public async Task<OrderOutputDto> UpdateOrder(int id, OrderInputDto model)
    {
      try
      {
        var order = await _orderPersistence.GetOrderByIdAsync(id) ?? throw new Exception("Não foi possível encontrar o pedido");
        await ValidateItemOrder(model);
        model.Id = order.Id;

        //adds itemOrder
        model.ItemOrders.ToList().ForEach((itemOrder) =>
        {
          if (itemOrder.Id == 0) _generalPersistence.Add(_mapper.Map<ItemOrder>(itemOrder));
        });

        _mapper.Map(model, order);
        _generalPersistence.Update<Order>(order);

        if (await _generalPersistence.SaveChangesAsync())
        {
          Order orderResponse = await _orderPersistence.GetOrderByIdAsync(order.Id)
          ?? throw new Exception("Não foi possível encontrar o pedido após atualização");

          return _mapper.Map<OrderOutputDto>(orderResponse);
        }

        throw new Exception("Não foi possível alterar o pedido");
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<OrderOutputDto> UpdateStep(int id, int step)
    {
      try
      {
        var order = await _orderPersistence.GetOrderByIdAsync(id) ?? throw new Exception("Pedido não encontrado.");

        order.Step = (Step)step;
        _generalPersistence.Update(order);

        if (await _generalPersistence.SaveChangesAsync())
        {
          Order orderResponse = await _orderPersistence.GetOrderByIdAsync(order.Id)
          ?? throw new Exception("Não foi possível encontrar o pedido após atualização");

          return _mapper.Map<OrderOutputDto>(orderResponse);
        }

        throw new Exception("Não foi possível alterar a etapa");
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteOrder(int id)
    {
      try
      {
        var order = await _orderPersistence.GetOrderByIdAsync(id);
        if (order == null) throw new Exception("Pedido não encontrado.");

        _generalPersistence.Delete<Order>(order);

        return await _generalPersistence.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public async Task<PageList<OrderOutputDto>> GetOrdersAsync(OrderParams orderParams)
    {
      try
      {
        if (orderParams.OrderedAtStart > orderParams.OrderedAtEnd) throw new Exception("Data de início não pode ser maior que a data final");

        PageList<Order> orders = await _orderPersistence.GetOrdersAsync(orderParams);

        PageList<OrderOutputDto> result = new()
        {
          CurrentPage = orders.CurrentPage,
          TotalPages = orders.TotalPages,
          PageSize = orders.PageSize,
          TotalCount = orders.TotalCount
        };

        result.AddRange(_mapper.Map<OrderOutputDto[]>(orders));

        return result;
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }


    public async Task<OrderOutputDto[]> GetPendingOrdersAsync()
    {
      try
      {
        var orders = await _orderPersistence.GetPendingOrdersAsync();

        var ordersDto = _mapper.Map<OrderOutputDto[]>(orders);

        return ordersDto;
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }


    public async Task<OrderOutputDto[]> GetPendingOrdersByExecutorAsync(int userId)
    {
      try
      {
        var orders = await _orderPersistence.GetPendingOrdersByExecutor(userId);

        var ordersDto = _mapper.Map<OrderOutputDto[]>(orders);

        return ordersDto;
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<OrderOutputDto> GetOrderByIdAsync(int id)
    {
      try
      {
        Order order = await _orderPersistence.GetOrderByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o pedido");
        OrderOutputDto orderDto = _mapper.Map<OrderOutputDto>(order);

        orderDto.Executor = _mapper.Map<UserOutputDto>(await _userPersistence.GetUserByIdAsync(order.ExecutorId));
        orderDto.Executor.Name = $"{orderDto.Executor.FirstName} {orderDto.Executor.LastName}";

        return orderDto;
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task ValidateItemOrder(OrderInputDto model)
    {
      foreach (var itemOrder in model.ItemOrders)
      {
        Item itemResponse = await _itemPersistence.GetItemByIdAsync(itemOrder.ItemId)
          ?? throw new Exception("Não foi encontrado o item a ser validao");

        if (itemResponse.ItemColors.FirstOrDefault(x => x.ColorId == itemOrder.ColorId) == null) throw new Exception($"Cor inválida no item de id: {itemOrder.ItemId}"); ;
        if (itemResponse.ItemFabrics.FirstOrDefault(x => x.FabricId == itemOrder.FabricId) == null) throw new Exception($"Tecido inválido no item de id: {itemOrder.ItemId}");
        if (itemResponse.ItemSizes.FirstOrDefault(x => x.SizeId == itemOrder.SizeId) == null) throw new Exception($"Tamanho inválido no item de id: {itemOrder.ItemId}");
      }
    }
  }
}