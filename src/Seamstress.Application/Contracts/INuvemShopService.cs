using Seamstress.Application.Dtos;

namespace Seamstress.Application.Contracts
{
    public interface INuvemShopService
    {
        Task<ImportPreviewDto> FetchAndPreviewAsync();
    }
}
