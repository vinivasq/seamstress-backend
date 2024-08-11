namespace Seamstress.DTO
{
  public class SetItemOutputSetDTO
  {
    public int SetId { get; set; }
    public SetOutputDTO Set { get; set; } = null!;
    public int ItemId { get; set; }
    public bool IsPrimary { get; set; } = false;
  }
}