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
    private readonly IMapper _mapper;


    public ItemOrderService(IGeneralPersistence generalPersistence,
                            IItemOrderPersistence itemOrderPersistence,
                            IItemPersistence itemPersistence,
                            IMapper mapper)
    {
      this._generalPersistence = generalPersistence;
      this._itemOrderPersistence = itemOrderPersistence;
      this._itemPersistence = itemPersistence;
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
          var itemOrderResponse = await _itemOrderPersistence.GetItemOrderByIdAsync(itemOrder.Id);
          return itemOrderResponse;
        }

        throw new Exception("Não foi possível adicionar o item ao pedido");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteItemOrder(int id)
    {
      try
      {
        ItemOrder itemOrder = await _itemOrderPersistence.GetItemOrderByIdAsync(id) ??
        throw new Exception("Não foi possível localizar o item a ser deletado");

        _generalPersistence.Delete(itemOrder);

        return await _generalPersistence.SaveChangesAsync();
      }
      catch (Exception ex)
      {

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
        var item = await _itemPersistence.GetItemByIdAsync(model.ItemId);

        if (item.ItemColors.FirstOrDefault(x => x.ColorId == model.ColorId) == null) throw new Exception("Cor inválida");
        if (item.ItemFabrics.FirstOrDefault(x => x.FabricId == model.FabricId) == null) throw new Exception("Tecido inválido");
        if (item.ItemSizes.FirstOrDefault(x => x.Id == model.SizeId) == null) throw new Exception("Tamanho inválido");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}