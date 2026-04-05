namespace Seamstress.Domain
{
  public class Item
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? SetId { get; set; }
    public Set? Set { get; set; }
    public IEnumerable<ItemColor> ItemColors { get; set; } = null!;
    public IEnumerable<ItemFabric> ItemFabrics { get; set; } = null!;
    public IEnumerable<ItemSize> ItemSizes { get; set; } = null!;
    public string ImageURL { get; set; } = null!;
    public decimal Price { get; set; }
    public bool? IsActive { get; set; } = true;
    public string? ExternalId { get; set; }
    public int? SalePlatformId { get; set; }
    public SalePlatform? SalePlatform { get; set; }
    public string? MeasurementsDescription { get; set; }
  }
}