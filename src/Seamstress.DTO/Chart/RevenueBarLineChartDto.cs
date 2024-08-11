namespace Seamstress.DTO
{
  public class RevenueBarLineChartDto
  {
    public List<DecimalDataSetDto> DataSets { get; set; } = new() { };
    public List<string> Labels { get; set; } = new() { };
  }
}