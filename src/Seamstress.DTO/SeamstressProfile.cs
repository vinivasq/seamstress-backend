using AutoMapper;
using Seamstress.Domain;
using Seamstress.Domain.Identity;
using Seamstress.DTO;
using Seamstress.ViewModel;

namespace Seamstress.Application.Helpers
{
  public class SeamstressProfile : Profile
  {
    public SeamstressProfile()
    {
      CreateMap<User, UserLoginDto>().ReverseMap();
      CreateMap<User, UserUpdateDto>().ReverseMap();
      CreateMap<User, UserOutputDto>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")).ReverseMap();
      CreateMap<Customer, CustomerDto>().ReverseMap();
      CreateMap<ItemSize, ItemSizeDto>().ReverseMap();
      CreateMap<ItemSize, ItemSizeForMeasurementsDto>().ReverseMap();
      CreateMap<ItemSizeMeasurement, ItemSizeMeasurementDto>().ReverseMap();
      CreateMap<Customer, CustomerOutputDto>();
      CreateMap<OrderInputDto, Order>();
      CreateMap<Order, OrderOutputDto>();
      CreateMap<ItemOrderInputDto, ItemOrder>();

      CreateMap<Set, SetOutputDto>();
      CreateMap<SetItemInputDto, SetItem>();
      CreateMap<SetItem, SetItemOutputSetDto>();
      CreateMap<ItemInputDto, Item>();
      CreateMap<Item, ItemOutputDto>()
        .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.ItemColors.Select(ic => ic.Color)))
        .ForMember(dest => dest.Fabrics, opt => opt.MapFrom(src => src.ItemFabrics.Select(ic => ic.Fabric)));
      CreateMap<User, UserDto>().ReverseMap();

      CreateMap<IntSalePlatformsDataSet, IntDataSetDto>()
      .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.SalePlatform.Name));
      CreateMap<DecimalSalePlatformsDataSet, DecimalDataSetDto>()
      .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.SalePlatform.Name));
      CreateMap<BarLineChart, BarLineChartDto>()
      .ForMember(dest => dest.DataSets, opt => opt.MapFrom(src => src.DataSets));
      CreateMap<RevenueBarLineChart, RevenueBarLineChartDto>()
      .ForMember(dest => dest.DataSets, opt => opt.MapFrom(src => src.DataSets));
    }
  }
}