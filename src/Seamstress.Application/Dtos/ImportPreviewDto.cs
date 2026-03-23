namespace Seamstress.Application.Dtos
{
  public class ImportPreviewDto
  {
    public string SessionId { get; set; } = null!;
    public List<ImportPreviewItemDto> ToCreate { get; set; } = new();
    public List<ImportPreviewItemDto> ToUpdate { get; set; } = new();
    public List<ImportPreviewItemDto> ToInactivate { get; set; } = new();
    public List<ImportPreviewItemDto> Failed { get; set; } = new();
  }
}
