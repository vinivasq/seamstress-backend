namespace Seamstress.Persistence.Parameters
{
  public class CustomerExportParams
  {
    public DateTime? FromCreatedAt { get; set; }
    public DateTime? ToCreatedAt { get; set; }
    public DateTime? FromOrderDate { get; set; }
    public DateTime? ToOrderDate { get; set; }
  }
}
