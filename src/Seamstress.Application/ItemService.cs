using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.DTO;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class ItemService : IItemService
  {
    private readonly IItemPersistence _itemPersistence;
    private readonly IItemSizePersistence _itemSizePersistence;
    private readonly IGeneralPersistence _generalPersistence;
    private readonly IAzureBlobService _azureService;
    private readonly IMapper _mapper;

    public ItemService(IItemPersistence itemPersistence,
                        IItemSizePersistence itemSizePersistence,
                        IAzureBlobService azureService,
                        IGeneralPersistence generalPersistence,
                        IMapper mapper)
    {
      this._mapper = mapper;
      this._generalPersistence = generalPersistence;
      this._azureService = azureService;
      this._itemPersistence = itemPersistence;
      this._itemSizePersistence = itemSizePersistence;
    }


    public async Task<ItemOutputDto> AddItem(ItemInputDto model)
    {
      try
      {
        var item = _mapper.Map<Item>(model);

        _generalPersistence.Add(item);

        if (await _generalPersistence.SaveChangesAsync())
        {
          var itemResponse = await _itemPersistence.GetItemByIdAsync(item.Id)
            ?? throw new Exception("Não foi possível encontrar o item após o cadastro.");

          return _mapper.Map<ItemOutputDto>(itemResponse);
        }

        throw new Exception("Não foi possível cadastrar o item");

      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<ItemOutputDto> UpdateITem(int id, ItemInputDto model)
    {
      try
      {
        var item = await _itemPersistence.GetItemByIdAsync(id) ?? throw new Exception("Não foi possível encontrar o item.");

        model.Id = item.Id;

        var itemColors = item.ItemColors.Select(x => x.ColorId).ToList();
        var itemFabrics = item.ItemFabrics.Select(x => x.FabricId).ToList();
        var itemSizes = item.ItemSizes.Select(x => x.SizeId).ToList();

        var modelColors = model.ItemColors.Select(x => x.ColorId).ToList();
        var modelFabrics = model.ItemFabrics.Select(x => x.FabricId).ToList();
        var modelSizes = model.ItemSizes.Select(x => x.SizeId).ToList();

        var colorsToRemove = item.ItemColors.Where(ic => itemColors.Except(modelColors).Contains(ic.ColorId)).ToArray();
        var fabricsToRemove = item.ItemFabrics.Where(IF => itemFabrics.Except(modelFabrics).Contains(IF.FabricId)).ToArray();
        var sizesToRemove = item.ItemSizes.Where(IS => itemSizes.Except(modelSizes).Contains(IS.SizeId)).ToArray();

        var colorsToAdd = modelColors.Except(itemColors).ToList();
        var fabricsToAdd = modelFabrics.Except(itemFabrics).ToList();
        var sizesToAdd = modelSizes.Except(itemSizes).ToList();

        if (colorsToRemove.Length > 0) _generalPersistence.DeleteRange(colorsToRemove);
        if (fabricsToRemove.Length > 0) _generalPersistence.DeleteRange(fabricsToRemove);
        if (sizesToRemove.Length > 0)
        {
          foreach (ItemSize itemSizeToRemove in sizesToRemove)
          {
            //Gets the ItemSize without item to prevent reference cycle and ItemSizeMeasurements deletition misbehaviour
            ItemSize itemSize = await _itemSizePersistence.GetOnlyItemSizeByIdAsync(itemSizeToRemove.Id);
            _generalPersistence.Delete(itemSize);
          }
        }

        if (colorsToAdd.Count > 0) colorsToAdd.ForEach((colorId) =>
        {
          ItemColor color = new()
          {
            ItemId = item.Id,
            ColorId = colorId,
          };

          _generalPersistence.Add(color);
        });

        if (fabricsToAdd.Count > 0) fabricsToAdd.ForEach((fabricId) =>
        {
          ItemFabric fabric = new()
          {
            ItemId = item.Id,
            FabricId = fabricId
          };

          _generalPersistence.Add(fabric);
        });

        if (sizesToAdd.Count > 0) sizesToAdd.ForEach((sizeId) =>
        {
          ItemSize modelItemSize = model.ItemSizes.First(x => x.SizeId == sizeId);

          ItemSize size = new()
          {
            ItemId = item.Id,
            SizeId = sizeId,
            Measurements = modelItemSize.Measurements
          };

          _generalPersistence.Add(size);
        });

        _mapper.Map(model, item);
        _generalPersistence.Update(item);

        if (await _generalPersistence.SaveChangesAsync())
        {
          var itemResponse = await _itemPersistence.GetItemByIdAsync(model.Id)
            ?? throw new Exception("Não foi possível encontrar o item após atualização.");
          return _mapper.Map<ItemOutputDto>(itemResponse);
        }

        throw new Exception("Não foi possível atualizar o item");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<ItemOutputDto> SetActiveState(int id, bool state)
    {
      try
      {
        var item = await _itemPersistence.GetItemByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o item informada.");

        item.IsActive = state;

        _generalPersistence.Update(item);

        if (await _generalPersistence.SaveChangesAsync())
        {
          var itemResponse = await _itemPersistence.GetItemByIdAsync(item.Id)
            ?? throw new Exception("Não foi possível listar o item após atualização.");
          return _mapper.Map<ItemOutputDto>(itemResponse);
        }

        throw new Exception("Não foi possível atualizar o item.");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> DeleteITem(int id)
    {
      try
      {
        var item = await _itemPersistence.GetItemWithoutAttributesAsync(id)
          ?? throw new Exception("Não foi possível encontrar o item a ser deletado.");
        List<string> images = item.ImageURL.Split(";").ToList();

        if (await _itemPersistence.CheckFKAsync(id) == false)
        {
          images.ForEach(image =>
          {
            this._azureService.DeleteModelImage(image);
          });

          _generalPersistence.Delete(item);

          return await _generalPersistence.SaveChangesAsync();
        }

        throw new Exception("Não é possível deletar pois existem registros vinculados");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<bool> CheckFK(int id)
    {
      try
      {
        var item = await _itemPersistence.GetItemWithoutAttributesAsync(id)
          ?? throw new Exception("Não foi possível encontrar o item a ser validado.");

        return await this._itemPersistence.CheckFKAsync(id);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public async Task<Item[]> GetItemsAsync()
    {
      try
      {
        return await _itemPersistence.GetAllItemsAsync();
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<Item> GetItemByIdAsync(int id)
    {
      try
      {
        return await _itemPersistence.GetItemByIdAsync(id)
          ?? throw new Exception("Não foi possível encontrar o item");
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}