using Seamstress.Domain;

namespace Seamstress.ViewModel
{
  public class IntSalePlatformsDataSet
  {
    public List<int> Data { get; set; } = new() { };
    public string Type { get; set; } = null!;
    public SalePlatform SalePlatform { get; set; } = null!;
  }
}