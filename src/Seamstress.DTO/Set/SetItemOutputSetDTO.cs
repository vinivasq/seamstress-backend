namespace Seamstress.DTO
{
  public class SetItemOutputSetDto
  {
    public int SetId { get; set; }
    public SetOutputDto Set { get; set; } = null!;
    public int ItemId { get; set; }
    public bool IsPrimary { get; set; } = false;
  }
}