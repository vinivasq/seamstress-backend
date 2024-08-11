using Seamstress.Domain;

namespace Seamstress.DTO
{
  public class ItemSizeForMeasurementsDto
  {
    public int Id { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int SizeId { get; set; }
    public Size? Size { get; set; }
    public IEnumerable<ItemSizeMeasurementDto>? Measurements { get; set; }
  }
}