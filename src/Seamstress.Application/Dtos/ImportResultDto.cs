namespace Seamstress.Application.Dtos
{
  public class ImportResultDto
  {
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Inactivated { get; set; }
    public List<ImportPreviewItemDto> Failed { get; set; } = new();
    public List<string> Errors { get; set; } = new();
  }
}
