namespace Seamstress.Domain
{
  public class ItemSizeMeasurement
  {
    public int Id { get; set; }
    public int ItemSizeId { get; set; }
    public ItemSize? ItemSize { get; set; }
    public string Measure { get; set; } = null!;
    public double Value { get; set; }
  }
}