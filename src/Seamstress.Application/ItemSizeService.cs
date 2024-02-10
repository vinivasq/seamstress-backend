using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
  public class ItemSizeService : IItemSizeService
  {
    private readonly IItemSizePersistence _itemSizePersistence;
    private readonly IGeneralPersistence _generalPersistence;
    private readonly IMapper _mapper;
    public ItemSizeService(IItemSizePersistence itemSizePersistence,
                            IGeneralPersistence generalPersistence,
                            IMapper mapper)
    {
      this._mapper = mapper;
      this._generalPersistence = generalPersistence;
      this._itemSizePersistence = itemSizePersistence;
    }

    public async Task<ItemSizeForMeasurementsDto[]> GetItemSizesByItemIdAsync(int id)
    {
      try
      {
        return this._mapper.Map<ItemSizeForMeasurementsDto[]>(await _itemSizePersistence.GetItemSizesByItemIdAsync(id));
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public ItemSizeDto GetItemSizeByItemOrder(int itemId, int sizeId)
    {
      try
      {
        return this._mapper.Map<ItemSizeDto>(_itemSizePersistence.GetItemSizeByItemOrder(itemId, sizeId));
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public async Task<ItemSizeForMeasurementsDto> GetItemSizeByIdAsync(int id)
    {
      try
      {
        return this._mapper.Map<ItemSizeForMeasurementsDto>(await _itemSizePersistence.GetItemSizeByIdAsync(id));
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }
    public async Task<ItemSizeForMeasurementsDto> UpdateItemSizeAsync(int id, ItemSize model)
    {
      try
      {
        ItemSize itemSize = await _itemSizePersistence.GetItemSizeByIdAsync(id);
        model.Id = itemSize.Id;

        List<ItemSizeMeasurement> modelMeasurements = model.Measurements!.ToList();
        List<ItemSizeMeasurement> itemMeasurements = itemSize.Measurements!.ToList();

        if (itemMeasurements.Count > 0)
        {
          List<ItemSizeMeasurement> measurementsToRemove = itemMeasurements.Where(itemMeasurement => !modelMeasurements
                                                                              .Any(modelMeasurement => modelMeasurement.Id == itemMeasurement.Id &&
                                                                                    modelMeasurement.Id > 0)).ToList();


          if (measurementsToRemove.Count > 0)
          {
            measurementsToRemove.ForEach(measurement =>
            {
              measurement.ItemSize = null;
              _generalPersistence.Delete(measurement);
            });
          }
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

        await _generalPersistence.SaveChangesAsync();

        return this._mapper.Map<ItemSizeForMeasurementsDto>(await _itemSizePersistence.GetItemSizeByIdAsync(model.Id));
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