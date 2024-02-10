namespace Seamstress.Application.Dtos
{
  public class ItemSizeMeasurementDto
  {
    public int Id { get; set; }
    public int ItemSizeId { get; set; }
    public string Measure { get; set; } = null!;
    public double Value { get; set; }
  }
}