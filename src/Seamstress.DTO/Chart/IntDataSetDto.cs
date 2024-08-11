namespace Seamstress.DTO
{
  public class IntDataSetDto
  {
    public List<int> Data { get; set; } = new() { };
    public string Type { get; set; } = null!;
    public string Label { get; set; } = null!;
  }
}