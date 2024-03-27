namespace Seamstress.Domain
{
  public class Color
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool? IsActive { get; set; } = true;
  }
}