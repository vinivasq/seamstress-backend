namespace Seamstress.Domain
{
  public class Fabric
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool? IsActive { get; set; } = true;
  }
}