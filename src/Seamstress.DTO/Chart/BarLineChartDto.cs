namespace Seamstress.DTO
{
  public class BarLineChartDto
  {
    public List<IntDataSetDto> DataSets { get; set; } = new() { };
    public List<string> Labels { get; set; } = new() { };
  }
}