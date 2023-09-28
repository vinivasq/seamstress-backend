namespace Seamstress.Domain
{
  public class ItemSize
  {
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int SizeId { get; set; }
    public Size? Size { get; set; }
  }
}