namespace Seamstress.DTO
{
  public class DecimalDataSetDto
  {
    public List<decimal> Data { get; set; } = new() { };
    public string Type { get; set; } = null!;
    public string Label { get; set; } = null!;
  }
}