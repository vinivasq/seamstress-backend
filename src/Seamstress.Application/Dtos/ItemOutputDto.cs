using Seamstress.Domain;

namespace Seamstress.Application.Dtos
{
  public class ItemOutputDto
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Set? Set { get; set; }
    public IEnumerable<Color> Colors { get; set; } = null!;
    public IEnumerable<Fabric> Fabrics { get; set; } = null!;
    public IEnumerable<Size> Sizes { get; set; } = null!;
    public string ImageURL { get; set; } = null!;
    public decimal Price { get; set; }
  }
}