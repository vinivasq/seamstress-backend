using Microsoft.AspNetCore.Http.Features;
using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class ItemSizeService : IItemSizeService
  {
    private readonly IItemSizePersistence _itemSizePersistence;
    private readonly IGeneralPersistence _generalPersistence;
    public ItemSizeService(IItemSizePersistence itemSizePersistence, IGeneralPersistence generalPersistence)
    {
      this._generalPersistence = generalPersistence;
      this._itemSizePersistence = itemSizePersistence;
    }

    public async Task<ItemSize[]> GetItemSizesByItemIdAsync(int id)
    {
      try
      {
        return await _itemSizePersistence.GetItemSizesByItemIdAsync(id);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public async Task<ItemSize> GetItemSizeByIdAsync(int id)
    {
      try
      {
        return await _itemSizePersistence.GetItemSizeByIdAsync(id);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public async Task<ItemSize> UpdateItemSizeAsync(int id, ItemSize model)
    {
      try
      {
        ItemSize itemSize = await _itemSizePersistence.GetItemSizeByIdAsync(id);
        model.Id = itemSize.Id;

        List<ItemSizeMeasurement> modelMeasurements = model.Measurements!.ToList();
        List<ItemSizeMeasurement> itemMeasurements = itemSize.Measurements!.ToList();

        if (itemMeasurements.Count > 0)
        {
          ItemSizeMeasurement[] measurementsToRemove = itemMeasurements.Where(itemMeasurement => !modelMeasurements
                                                                              .Any(modelMeasurement => modelMeasurement.Id == itemMeasurement.Id &&
                                                                                    modelMeasurement.Id > 0)).ToArray();

          if (measurementsToRemove.Length > 0)
            _generalPersistence.DeleteRange(measurementsToRemove);
        }

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

        if (modelMeasurements.Count == 0 && itemMeasurements.Count > 0)
          _generalPersistence.DeleteRange(itemMeasurements.ToArray());

        await _generalPersistence.SaveChangesAsync();
        return await _itemSizePersistence.GetItemSizeByIdAsync(model.Id);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public async Task<bool> DeleteItemSizeAsync(int id)
    {
      try
      {
        var itemSize = await _itemSizePersistence.GetItemSizeByIdAsync(id);
        _generalPersistence.Delete(itemSize);

        return await _generalPersistence.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
  }
}