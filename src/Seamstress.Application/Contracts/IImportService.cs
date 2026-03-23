using Seamstress.Application.Dtos;

namespace Seamstress.Application.Contracts
{
    public interface IImportService
    {
        Task<ImportPreviewDto> GeneratePreviewAsync(List<NormalizedProduct> products, int salePlatformId);
        Task<ImportResultDto> ExecuteImportAsync(string sessionId);
    }
}
