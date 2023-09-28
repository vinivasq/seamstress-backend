using Seamstress.Domain;

namespace Seamstress.Application.Dtos
{
  public class ItemInputDto
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? SetId { get; set; }
    public IEnumerable<ItemColor> ItemColors { get; set; } = null!;
    public IEnumerable<ItemFabric> ItemFabrics { get; set; } = null!;
    public IEnumerable<ItemSize> ItemSizes { get; set; } = null!;
    public string? ImageURL { get; set; }
    public decimal Price { get; set; }
  }
}