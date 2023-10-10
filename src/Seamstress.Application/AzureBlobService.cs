using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Seamstress.Application.Contracts;

namespace Seamstress.Application
{
  public class AzureBlobService : IAzureBlobService
  {
    private readonly string _storageAccount = "seamstressstorage";
    private readonly string _accessKey = "D8M6kVO/OTC3v3MJus++GfJUrS3CUte0wsSiwBAqRFOqis+M+qRbpMMNl6Uot7HI0rgi9MKRiQ45+AStAFHGqQ==";

    private BlobServiceClient GetServiceClient()
    {
      try
      {
        StorageSharedKeyCredential credential = new(_storageAccount, _accessKey);
        string blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        return new BlobServiceClient(new Uri(blobUri), credential);
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao recuperar as credenciais do Azure. Erro: {ex.Message}");
      }
    }

    public async Task<string> ListBlobContainersAsync()
    {
      BlobServiceClient client = GetServiceClient();
      var containers = client.GetBlobContainersAsync();
      List<string> names = new();

      await foreach (var container in containers)
      {
        names.Add(container.Name);
      }

      return string.Join(",", names);
    }

    public async Task<string> UploadModelImageAsync(IFormFile imageFile, string imageName)
    {
      BlobServiceClient client = GetServiceClient();
      try
      {
        BlobContainerClient blobContainer = client.GetBlobContainerClient("images");
        Stream file = imageFile.OpenReadStream();

        var blob = blobContainer.GetBlobClient($"models/{imageName}");
        await blob.UploadAsync(file);

        return blob.Name.Split("/")[1];
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao gravar imagens do modelo. Erro: {ex.Message}");
      }
    }

    public bool DeleteModelImage(string imageName)
    {
      BlobServiceClient client = GetServiceClient();

      try
      {
        BlobContainerClient blobContainer = client.GetBlobContainerClient("images");

        var blob = blobContainer.GetBlobClient($"models/{imageName}");

        return blob.DeleteIfExists();
      }
      catch (Exception ex)
      {

        throw new Exception($"Não foi possível deletar a imagem do modelo. Erro: {ex.Message}");
      }

    }
  }
}