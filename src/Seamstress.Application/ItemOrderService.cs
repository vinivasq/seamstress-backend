using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class ItemOrderService : IItemOrderService
  {
    private readonly IGeneralPersistence _generalPersistence;
    private readonly IItemOrderPersistence _itemOrderPersistence;
    private readonly IItemPersistence _itemPersistence;
    private readonly IOrderPersistence _orderPersistence;
    private readonly IMapper _mapper;


    public ItemOrderService(IGeneralPersistence generalPersistence,
                            IItemOrderPersistence itemOrderPersistence,
                            IItemPersistence itemPersistence,
                            IOrderPersistence orderPersistence,
                            IMapper mapper)
    {
      this._generalPersistence = generalPersistence;
      this._itemOrderPersistence = itemOrderPersistence;
      this._itemPersistence = itemPersistence;
      this._orderPersistence = orderPersistence;
      this._mapper = mapper;

    }


    public async Task<ItemOrder> AddItemOrder(ItemOrderInputDto model)
    {
      try
      {
        await ValidateItemOrder(model);

        ItemOrder itemOrder = _mapper.Map<ItemOrder>(model);

        _generalPersistence.Add(itemOrder);

        if (await _generalPersistence.SaveChangesAsync())
        {
          var itemOrderResponse = await _itemOrderPersistence.GetItemOrderByIdAsync(itemOrder.Id)
          ?? throw new Exception("Erro ao recuperar o item de pedido após o cadastro");
          return itemOrderResponse;
        }

        throw new Exception("Não foi possível adicionar o item ao pedido");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<ItemOrder> UpdateItemOrder(int id, ItemOrderInputDto model)
    {
      try
      {
        await ValidateItemOrder(model);
        ItemOrder itemOrder = await this._itemOrderPersistence.GetItemOrderByIdAsync(id)
          ?? throw new Exception("Não foi possível recuperar o item de pedido a ser alterado");

        _generalPersistence.BeginTransaction();

        _mapper.Map(model, itemOrder);
        _generalPersistence.Update(itemOrder);

        if (await _generalPersistence.SaveChangesAsync())
        {
          ItemOrder itemOrderResponse = await this._itemOrderPersistence.GetItemOrderByIdAsync(itemOrder.Id)
            ?? throw new Exception("Não foi possível recuperar o item de pedido após alteração");

          Order order = await this._orderPersistence.GetOrderByIdAsync(itemOrderResponse.OrderId)
            ?? throw new Exception("Não foi possível recuperar o  pedido a ser alterado");

          decimal total = 0;
          foreach (var item in order.ItemOrders)
          {
            total += item.Item!.Price * item.Amount;
          }

          order.Total = total;

          this._generalPersistence.Update(order);

          if (this._generalPersistence.SaveChanges() == false)
            throw new Exception("Erro ao atualizar o total do pedido");

          this._generalPersistence.CommitTransaction();
          return itemOrderResponse;
        }

        throw new Exception("Não foi possível salvar as alterações no item de pedido");
      }
      catch (Exception ex)
      {
        this._generalPersistence.RollbackTransaction();
        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteItemOrder(int id)
    {
      try
      {
        ItemOrder itemOrder = await _itemOrderPersistence.GetItemOrderByIdAsync(id)
          ?? throw new Exception("Não foi possível localizar o item a ser deletado");

        _generalPersistence.BeginTransaction();
        _generalPersistence.Delete(itemOrder);

        if (await _generalPersistence.SaveChangesAsync())
        {
          Order order = await this._orderPersistence.GetOrderByIdAsync(itemOrder.OrderId)
            ?? throw new Exception("Não foi possível recuperar o  pedido a ser alterado");

          decimal total = 0;
          foreach (var item in order.ItemOrders)
          {
            total += item.Item!.Price * item.Amount;
          }

          order.Total = total;

          this._generalPersistence.Update(order);

          if (this._generalPersistence.SaveChanges() == false)
            throw new Exception("Erro ao atualizar o total do pedido");

          this._generalPersistence.CommitTransaction();
          return true;
        };

        throw new Exception("Não foi possível salvar as alterações no item de pedido");
      }
      catch (Exception ex)
      {
        this._generalPersistence.RollbackTransaction();
        throw new Exception(ex.Message);
      }
    }

    public async Task<ItemOrderOutputDto> GetItemOrderById(int id)
    {
      try
      {
        ItemOrder itemOrder = await _itemOrderPersistence.GetItemOrderByIdAsync(id) ??
        throw new Exception("Não foi possível localizar o item");

        return _mapper.Map<ItemOrderOutputDto>(itemOrder);
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<ItemOrderOutputDto[]> GetItemOrdersByOrderId(int orderId)
    {
      try
      {
        ItemOrder[] itemOrders = await _itemOrderPersistence.GetItemOrdersByOrderIdAsync(orderId) ??
        throw new Exception("Não foi possível localizar os itens");

        List<ItemOrderOutputDto> itemOrdersDto = new();

        itemOrders.ToList().ForEach((itemOrder) =>
        {
          itemOrdersDto.Add(_mapper.Map<ItemOrderOutputDto>(itemOrder));
        });

        return itemOrdersDto.ToArray();
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task ValidateItemOrder(ItemOrderInputDto model)
    {
      try
      {
        var item = await _itemPersistence.GetItemByIdAsync(model.ItemId)
          ?? throw new Exception("Não foi encontrado o item a ser validao");

        if (item.ItemColors.FirstOrDefault(x => x.ColorId == model.ColorId) == null) throw new Exception("Cor inválida");
        if (item.ItemFabrics.FirstOrDefault(x => x.FabricId == model.FabricId) == null) throw new Exception("Tecido inválido");
        if (item.ItemSizes.FirstOrDefault(x => x.SizeId == model.SizeId) == null) throw new Exception("Tamanho inválido");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}