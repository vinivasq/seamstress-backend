namespace Seamstress.Domain
{
  public class SetItem
  {
    public int SetId { get; set; }
    public Set Set { get; set; } = null!;
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
  }
}