using Seamstress.Domain;

namespace Seamstress.Persistence.Models.ViewModels.Charts
{
  public class IntSalePlatformsDataSet
  {
    public List<int> Data { get; set; } = new() { };
    public string Type { get; set; } = null!;
    public SalePlatform SalePlatform { get; set; } = null!;
  }
}