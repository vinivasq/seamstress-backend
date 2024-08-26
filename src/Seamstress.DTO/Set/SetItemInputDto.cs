namespace Seamstress.DTO
{
  public class SetItemInputDto
  {
    public int SetId { get; set; }
    public int ItemId { get; set; }
    public bool IsPrimary { get; set; } = false;
  }
}