using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Domain.Enum;
using Seamstress.Persistence.Contracts;

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
          var orderResponse = await _orderPersistence.GetOrderByIdAsync(order.Id);

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
          var orderResponse = await _orderPersistence.GetOrderByIdAsync(order.Id);

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
          var orderResponse = await _orderPersistence.GetOrderByIdAsync(order.Id);

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
    public async Task<OrderOutputDto[]> GetAllOrdersAsync()
    {
      try
      {
        var orders = await _orderPersistence.GetAllOrdersAsync();

        var ordersDto = _mapper.Map<OrderOutputDto[]>(orders);

        return ordersDto;
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

    public async Task<OrderOutputDto[]> GetOrdersByExecutorAsync(int userId)
    {
      try
      {
        var orders = await _orderPersistence.GetOrdersByExecutor(userId);

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
        var order = await _orderPersistence.GetOrderByIdAsync(id);
        var orderDto = _mapper.Map<OrderOutputDto>(order);

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
        var itemResponse = await _itemPersistence.GetItemByIdAsync(itemOrder.ItemId);

        if (itemResponse.ItemColors.FirstOrDefault(x => x.ColorId == itemOrder.ColorId) == null) throw new Exception($"Cor inválida no item de id: {itemOrder.ItemId}"); ;
        if (itemResponse.ItemFabrics.FirstOrDefault(x => x.FabricId == itemOrder.FabricId) == null) throw new Exception($"Tecido inválido no item de id: {itemOrder.ItemId}");
        if (itemResponse.ItemSizes.FirstOrDefault(x => x.Id == itemOrder.SizeId) == null) throw new Exception($"Tamanho inválido no item de id: {itemOrder.ItemId}");
      }
    }
  }
}