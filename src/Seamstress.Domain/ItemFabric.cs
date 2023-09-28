namespace Seamstress.Domain
{
  public class ItemFabric
  {
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int FabricId { get; set; }
    public Fabric? Fabric { get; set; }
  }
}