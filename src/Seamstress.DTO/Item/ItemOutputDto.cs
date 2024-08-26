using Seamstress.Domain;

namespace Seamstress.DTO
{
  public class ItemOutputDto
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public SetItemOutputSetDto? SetItem { get; set; }
    public IEnumerable<Color> Colors { get; set; } = null!;
    public IEnumerable<Fabric> Fabrics { get; set; } = null!;
    public IEnumerable<ItemSizeDto> ItemSizes { get; set; } = null!;
    public string ImageURL { get; set; } = null!;
    public bool IsActive { get; set; }
    public decimal Price { get; set; }
  }
}