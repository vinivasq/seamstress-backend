namespace Seamstress.Application.Dtos
{
  public class BarLineChartDto
  {
    public List<DataSetDto> DataSets { get; set; } = new() { };
    public List<string> Labels { get; set; } = new() { };
  }
  public class DataSetDto
  {
    public List<int> Data { get; set; } = new() { };
    public string Type { get; set; } = null!;
    public string Label { get; set; } = null!;
  }
}