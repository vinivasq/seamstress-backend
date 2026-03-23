namespace Seamstress.Application.Dtos
{
  public class ImportPreviewItemDto
  {
    public string ExternalId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public List<string> Colors { get; set; } = new();
    public string Fabric { get; set; } = null!;
    public List<string> Sizes { get; set; } = new();
    public string Action { get; set; } = null!;  // "Create", "Update", "Inactivate", "Failed"
    public List<string> Changes { get; set; } = new();  // For updates: what changed
    public string? FailReason { get; set; }  // For failed: why it was skipped
  }
}
