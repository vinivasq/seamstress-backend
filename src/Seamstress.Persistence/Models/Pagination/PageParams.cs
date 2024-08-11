namespace Seamstress.Persistence.Models
{
  public class PageParams
  {
    public const int MaxPageSize = 100;
    public int PageNumber { get; set; } = 0;
    private int pageSize = 25;
    public int PageSize
    {
      get { return pageSize; }
      set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
    }
    public string Term { get; set; } = string.Empty;

  }
}