namespace Seamstress.Domain
{
  public class Size
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool? IsActive { get; set; } = true;
  }
}