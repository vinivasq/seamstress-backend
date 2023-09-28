using Seamstress.Domain;

namespace Seamstress.Application.Dtos
{
  public class ItemOrderOutputDto
  {
    public int Id { get; set; }
    public Color Color { get; set; } = null!;
    public Fabric Fabric { get; set; } = null!;
    public Size Size { get; set; } = null!;
    public Sizings? AditionalSizing { get; set; }
    public int OrderId { get; set; }
    public string? Description { get; set; }
    public int Amount { get; set; }

    public int ItemId { get; set; }
    public ItemOutputDto Item { get; set; } = null!;
  }
}