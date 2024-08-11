namespace Seamstress.Persistence.Models
{
  public class OrderParams : PageParams
  {
    public int? CustomerId { get; set; }
    public DateTime OrderedAtStart { get; set; }
    public DateTime OrderedAtEnd { get; set; }
    public int[]? Steps { get; set; }
  }
}