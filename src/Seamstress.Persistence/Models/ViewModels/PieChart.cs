namespace Seamstress.Persistence.Models.ViewModels
{
  public class PieChart
  {
    public List<int> DataSets { get; set; } = new() { };
    public List<string> Labels { get; set; } = new() { };
  }
}