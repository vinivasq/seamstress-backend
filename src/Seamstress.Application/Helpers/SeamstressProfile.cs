using AutoMapper;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Domain.Identity;

namespace Seamstress.Application.Helpers
{
  public class SeamstressProfile : Profile
  {
    private readonly IItemSizeService _itemSizeService;
    public SeamstressProfile(IItemSizeService itemSizeService)
    {
      this._itemSizeService = itemSizeService;

      CreateMap<Customer, CustomerDto>().ReverseMap();
      CreateMap<Customer, CustomerOutputDto>();
      CreateMap<OrderInputDto, Order>();
      CreateMap<Order, OrderOutputDto>();
      CreateMap<ItemOrderInputDto, ItemOrder>();
      CreateMap<ItemOrder, ItemOrderOutputDto>()
        .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
        .ForMember(dest => dest.Fabric, opt => opt.MapFrom(src => src.Fabric))
        .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
        .ForMember(dest => dest.ItemSize, opt => opt.MapFrom(src => _itemSizeService.GetItemSizeByItemOrder(src.ItemId, src.SizeId)));
      CreateMap<ItemInputDto, Item>();
      CreateMap<ItemSize, ItemSizeDto>();
      CreateMap<Item, ItemOutputDto>()
        .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.ItemColors.Select(ic => ic.Color)))
        .ForMember(dest => dest.Fabrics, opt => opt.MapFrom(src => src.ItemFabrics.Select(ic => ic.Fabric)));
      CreateMap<User, UserDto>().ReverseMap();
      CreateMap<User, UserLoginDto>().ReverseMap();
      CreateMap<User, UserUpdateDto>().ReverseMap();
      CreateMap<User, UserOutputDto>().ReverseMap();
    }
  }
}