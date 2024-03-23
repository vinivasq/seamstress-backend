using Seamstress.Domain;

namespace Seamstress.Persistence.Models.ViewModels
{
  public class BarLineChart
  {
    public List<DataSet> DataSets { get; set; } = new() { };
    public List<string> Labels { get; set; } = null!;
  }
  public class DataSet
  {
    public List<int> Data { get; set; } = new() { };
    public string Type { get; set; } = null!;
    public SalePlatform SalePlatform { get; set; } = null!;
  }

}