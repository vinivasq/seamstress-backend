using Microsoft.AspNetCore.Http;
using Seamstress.Application.Dtos;
using Seamstress.Domain;

namespace Seamstress.Application.Contracts
{
  public interface IImportService
  {
    Task<ImportUploadResultDto> ParseCsvAsync(IFormFile file, int salePlatformId);
    Task<ImportPreviewDto> GeneratePreviewAsync(string sessionId, List<ImportColumnMappingDto> mappings, int salePlatformId);
    Task<ImportResultDto> ExecuteImportAsync(string sessionId);
    Task<ImportMapping?> GetMappingAsync(int salePlatformId);
    Task<ImportMapping> SaveMappingAsync(int salePlatformId, List<ImportColumnMappingDto> mappings);
  }
}
