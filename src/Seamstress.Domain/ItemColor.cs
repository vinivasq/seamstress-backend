namespace Seamstress.Domain
{
  public class ItemColor
  {
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int ColorId { get; set; }
    public Color? Color { get; set; }
  }
}