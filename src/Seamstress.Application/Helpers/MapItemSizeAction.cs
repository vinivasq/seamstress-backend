using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Domain;
using Seamstress.DTO;

namespace Seamstress.Application.Helpers
{
  public class MapItemSizeAction : IMappingAction<ItemOrder, ItemOrderOutputDto>
  {
    private readonly IItemSizeService _itemSizeService;

    public MapItemSizeAction(IItemSizeService itemSizeService)
    {
      this._itemSizeService = itemSizeService;
    }

    public void Process(ItemOrder source, ItemOrderOutputDto destination, ResolutionContext context)
    {
      var itemSize = _itemSizeService.GetItemSizeByItemOrder(source.ItemId, source.SizeId);

      destination.ItemSize = context.Mapper.Map<ItemSizeDto>(itemSize);
    }
  }
}