using Seamstress.Domain;

namespace Seamstress.Persistence.Models.ViewModels.Charts
{
  public class DecimalSalePlatformsDataSet
  {
    public List<decimal> Data { get; set; } = new() { };
    public string Type { get; set; } = null!;
    public SalePlatform SalePlatform { get; set; } = null!;
  }
}