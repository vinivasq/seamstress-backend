using Seamstress.Domain;
using Seamstress.DTO;

namespace Seamstress.Application.Helpers
{
  public class SeamstressApplicationProfile : SeamstressProfile
  {
    public SeamstressApplicationProfile()
    {
      CreateMap<ItemOrder, ItemOrderOutputDto>()
        .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
        .ForMember(dest => dest.Fabric, opt => opt.MapFrom(src => src.Fabric))
        .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
        .ForMember(dest => dest.ItemSize, opt => opt.Ignore())
        .AfterMap<MapItemSizeAction>();
    }
  }
}