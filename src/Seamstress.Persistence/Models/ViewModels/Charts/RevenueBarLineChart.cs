using Seamstress.Persistence.Models.ViewModels.Charts;

namespace Seamstress.Persistence.Models.ViewModels
{
  public class RevenueBarLineChart
  {
    public List<DecimalSalePlatformsDataSet> DataSets { get; set; } = new() { };
    public List<string> Labels { get; set; } = new() { };
  }
}