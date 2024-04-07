using Seamstress.Persistence.Models.ViewModels.Charts;

namespace Seamstress.Persistence.Models.ViewModels
{
  public class BarLineChart
  {
    public List<IntSalePlatformsDataSet> DataSets { get; set; } = new() { };
    public List<string> Labels { get; set; } = new() { };
  }
}