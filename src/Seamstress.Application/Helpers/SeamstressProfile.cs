using AutoMapper;
using Seamstress.Application.Dtos;
using Seamstress.Domain;
using Seamstress.Domain.Identity;
using Seamstress.Persistence.Models.ViewModels;

namespace Seamstress.Application.Helpers
{
  public class SeamstressProfile : Profile
  {
    public SeamstressProfile()
    {
      CreateMap<User, UserLoginDto>().ReverseMap();
      CreateMap<User, UserUpdateDto>().ReverseMap();
      CreateMap<User, UserOutputDto>().ReverseMap();
      CreateMap<Customer, CustomerDto>().ReverseMap();
      CreateMap<ItemSize, ItemSizeDto>().ReverseMap();
      CreateMap<ItemSize, ItemSizeForMeasurementsDto>().ReverseMap();
      CreateMap<ItemSizeMeasurement, ItemSizeMeasurementDto>().ReverseMap();
      CreateMap<Customer, CustomerOutputDto>();
      CreateMap<OrderInputDto, Order>();
      CreateMap<Order, OrderOutputDto>();
      CreateMap<ItemOrderInputDto, ItemOrder>();
      CreateMap<ItemOrder, ItemOrderOutputDto>()
        .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
        .ForMember(dest => dest.Fabric, opt => opt.MapFrom(src => src.Fabric))
        .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
        .ForMember(dest => dest.ItemSize, opt => opt.Ignore())
        .AfterMap<MapItemSizeAction>();


      CreateMap<ItemInputDto, Item>();
      CreateMap<Item, ItemOutputDto>()
        .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.ItemColors.Select(ic => ic.Color)))
        .ForMember(dest => dest.Fabrics, opt => opt.MapFrom(src => src.ItemFabrics.Select(ic => ic.Fabric)));
      CreateMap<User, UserDto>().ReverseMap();
      CreateMap<DataSet, DataSetDto>()
      .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.SalePlatform.Name));
      CreateMap<BarLineChart, BarLineChartDto>()
      .ForMember(dest => dest.DataSets, opt => opt.MapFrom(src => src.DataSets));
    }
  }
}