namespace Seamstress.Application.Dtos
{
  public class ImportUploadResultDto
  {
    public string SessionId { get; set; } = null!;
    public List<string> Columns { get; set; } = new();
    public List<Dictionary<string, string>> SampleRows { get; set; } = new();
    public List<ImportColumnMappingDto>? SavedMapping { get; set; }
  }
}
