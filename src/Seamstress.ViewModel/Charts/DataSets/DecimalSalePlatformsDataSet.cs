using Seamstress.Domain;

namespace Seamstress.ViewModel
{
  public class DecimalSalePlatformsDataSet
  {
    public List<decimal> Data { get; set; } = new() { };
    public string Type { get; set; } = null!;
    public SalePlatform SalePlatform { get; set; } = null!;
  }
}