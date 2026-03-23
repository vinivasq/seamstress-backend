namespace Seamstress.Application.Dtos
{
  public class ImportColumnMappingDto
  {
    public string CsvColumn { get; set; } = null!;
    public string SeamstressField { get; set; } = null!;
  }
}
