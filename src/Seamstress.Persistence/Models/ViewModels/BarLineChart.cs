namespace Seamstress.Persistence.Models.ViewModels
{
  public class BarLineChart
  {
    public IEnumerable<DataSet> DataSets { get; set; } = null!;
    public List<string> Labels { get; set; } = null!;
  }
  public class DataSet
  {
    public IEnumerable<int> Data { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Label { get; set; } = null!;
  }

}