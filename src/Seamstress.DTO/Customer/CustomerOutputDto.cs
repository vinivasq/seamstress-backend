using Seamstress.Domain;

namespace Seamstress.DTO
{
  public class CustomerOutputDto
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Sizings? Sizings { get; set; }
  }
}