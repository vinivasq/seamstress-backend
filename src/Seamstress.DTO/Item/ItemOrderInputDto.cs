namespace Seamstress.DTO
{
  public class ItemOrderInputDto
  {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ColorId { get; set; }
    public int FabricId { get; set; }
    public int SizeId { get; set; }
    public string? Description { get; set; }
    public int Amount { get; set; }

    public int ItemId { get; set; }
  }
}