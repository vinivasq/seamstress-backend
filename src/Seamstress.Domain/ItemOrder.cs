namespace Seamstress.Domain
{
  public class ItemOrder
  {
    public int Id { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ColorId { get; set; }
    public Color? Color { get; set; }
    public int FabricId { get; set; }
    public Fabric? Fabric { get; set; }
    public int? SizeId { get; set; }
    public Size? Size { get; set; }
    public int? ItemSizeId { get; set; }
    public ItemSize? ItemSize { get; set; }
    public string? Description { get; set; }
    public int Amount { get; set; }
  }
}