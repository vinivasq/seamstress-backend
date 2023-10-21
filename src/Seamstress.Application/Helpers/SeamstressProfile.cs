using AutoMapper;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Domain.Identity;

namespace Seamstress.Application.Helpers
{
  public class SeamstressProfile : Profile
  {
    public SeamstressProfile()
    {
      CreateMap<Customer, CustomerDto>().ReverseMap();
      CreateMap<OrderInputDto, Order>();
      CreateMap<Order, OrderOutputDto>();
      CreateMap<ItemOrderInputDto, ItemOrder>();
      CreateMap<ItemOrder, ItemOrderOutputDto>()
        .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
        .ForMember(dest => dest.Fabric, opt => opt.MapFrom(src => src.Fabric))
        .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size));
      CreateMap<ItemInputDto, Item>();
      CreateMap<Item, ItemOutputDto>()
        .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.ItemColors.Select(ic => ic.Color)))
        .ForMember(dest => dest.Fabrics, opt => opt.MapFrom(src => src.ItemFabrics.Select(ic => ic.Fabric)))
        .ForMember(dest => dest.Sizes, opt => opt.MapFrom(src => src.ItemSizes.Select(ic => ic.Size)));
      CreateMap<User, UserDto>().ReverseMap();
      CreateMap<User, UserLoginDto>().ReverseMap();
      CreateMap<User, UserUpdateDto>().ReverseMap();
      CreateMap<User, UserOutputDto>().ReverseMap();
    }
  }
}