using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class ItemService : IItemService
  {
    private readonly IItemPersistence _itemPersistence;
    private readonly IGeneralPersistence _generalPersistence;
    private readonly IAzureBlobService _azureService;
    private readonly IMapper _mapper;

    public ItemService(IItemPersistence itemPersistence,
                        IAzureBlobService azureService,
                        IGeneralPersistence generalPersistence,
                        IMapper mapper)
    {
      this._mapper = mapper;
      this._generalPersistence = generalPersistence;
      this._azureService = azureService;
      this._itemPersistence = itemPersistence;
    }


    public async Task<ItemOutputDto> AddItem(ItemInputDto model)
    {
      try
      {
        var item = _mapper.Map<Item>(model);

        _generalPersistence.Add(item);

        if (await _generalPersistence.SaveChangesAsync())
        {
          var itemResponse = await _itemPersistence.GetItemByIdAsync(item.Id);

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
        if (sizesToRemove.Length > 0) _generalPersistence.DeleteRange(sizesToRemove);

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
          List<ItemSizeMeasurement> modelMeasurements = model.ItemSizes.Where(x => x.Measurements != null && x.Id > 0).SelectMany(x => x.Measurements).ToList();

          var itemFromDB = await _itemPersistence.GetItemByIdAsync(id);

          List<ItemSizeMeasurement> itemMeasurements = itemFromDB.ItemSizes.Where(x => x.Measurements != null).SelectMany(x => x.Measurements).ToList();

          ItemSizeMeasurement[] measurementsToRemove = itemMeasurements.Where(itemMeasurement => !modelMeasurements.Any(modelMeasurement => modelMeasurement.Id == itemMeasurement.Id)).ToArray();


          if (modelMeasurements.Count > 0)
          {
            modelMeasurements.ForEach(measurement =>
            {
              if (measurement.Id == 0)
              {
                _generalPersistence.Add(measurement);
              }
              else
              {
                _generalPersistence.Update(measurement);
              }
            });
          }

          if (measurementsToRemove.Length > 0)
            _generalPersistence.DeleteRange(measurementsToRemove);

          if (await _generalPersistence.SaveChangesAsync())
          {
            var itemResponse = await _itemPersistence.GetItemByIdAsync(model.Id);

            return _mapper.Map<ItemOutputDto>(itemResponse);
          }
          throw new Exception("Não foi possível atualizar as medidas do item.");

        }

        throw new Exception("Não foi possível atualizar o item");
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
        var item = await _itemPersistence.GetItemByIdAsync(id) ?? throw new Exception("Não foi possível encontrar o item a ser deletado.");
        List<string> images = item.ImageURL.Split(";").ToList();

        images.ForEach(image =>
        {
          this._azureService.DeleteModelImage(image);
        });

        _generalPersistence.Delete(item);

        return await _generalPersistence.SaveChangesAsync();
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
        return await _itemPersistence.GetItemByIdAsync(id);
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }
  }
}