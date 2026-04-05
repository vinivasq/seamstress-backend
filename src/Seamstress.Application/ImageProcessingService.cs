using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seamstress.Application.Contracts;
using Seamstress.Persistence.Contracts;

namespace Seamstress.Application
{
    public class ImageProcessingService : BackgroundService
    {
        private readonly ImageProcessingQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ImageProcessingService> _logger;

        public ImageProcessingService(
            ImageProcessingQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<ImageProcessingService> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var job = await _queue.DequeueAsync(stoppingToken);
                    await ProcessImagesAsync(job);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar imagens em segundo plano");
                }
            }
        }

        private async Task ProcessImagesAsync(ImageProcessingJob job)
        {
            using var scope = _scopeFactory.CreateScope();
            var itemPersistence = scope.ServiceProvider.GetRequiredService<IItemPersistence>();
            var azureBlobService = scope.ServiceProvider.GetRequiredService<IAzureBlobService>();
            var generalPersistence = scope.ServiceProvider.GetRequiredService<IGeneralPersistence>();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();

            var allItems = await itemPersistence.GetItemsByExternalSourceAsync(job.SalePlatformId);

            foreach (var item in allItems.Where(i => i.IsActive == true && i.ExternalId != null && job.ChangedExternalIds.Contains(i.ExternalId!)))
            {
                if (!job.Products.TryGetValue(item.ExternalId!, out var product)) continue;
                if (product.ImageUrls.Count == 0) continue;

                var existingBlobs = string.IsNullOrEmpty(item.ImageURL)
                    ? new List<string>()
                    : item.ImageURL.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(b => b.Trim()).ToList();

                var blobNames = new List<string>();
                foreach (var imageUrl in product.ImageUrls)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(imageUrl)) continue;
                        var url = imageUrl.StartsWith("http") ? imageUrl : $"https:{imageUrl}";

                        using var response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        using var stream = await response.Content.ReadAsStreamAsync();

                        var extension = Path.GetExtension(new Uri(url).AbsolutePath);
                        if (string.IsNullOrEmpty(extension)) extension = ".jpg";
                        var imageName = $"{Guid.NewGuid()}{extension}";

                        var blobName = await azureBlobService.UploadModelImageAsync(stream, imageName);
                        blobNames.Add(blobName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Erro ao baixar imagem para {ItemName}", item.Name);
                    }
                }

                if (blobNames.Count > 0)
                {
                    foreach (var oldBlob in existingBlobs)
                    {
                        azureBlobService.DeleteModelImage(oldBlob);
                    }

                    item.ImageURL = string.Join(";", blobNames);
                    generalPersistence.Update(item);
                }
            }

            await generalPersistence.SaveChangesAsync();
            _logger.LogInformation("Processamento de imagens concluído para SalePlatformId {SalePlatformId}", job.SalePlatformId);
        }
    }
}
