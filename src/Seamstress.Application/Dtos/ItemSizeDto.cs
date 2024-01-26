using Seamstress.Domain;

namespace Seamstress.Application.Dtos
{
  public class ItemSizeDto
  {
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int SizeId { get; set; }
    public Size? Size { get; set; }
    public IEnumerable<ItemSizeMeasurement>? Measurements { get; set; }
  }
}