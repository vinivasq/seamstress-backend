using Microsoft.AspNetCore.Http;

namespace Seamstress.Application.Contracts
{
  public interface IAzureBlobService
  {
    public Task<string> ListBlobContainersAsync();
    public Task<string> UploadModelImageAsync(IFormFile imageFile, string imageName);
    public Task<string> UploadModelImageAsync(Stream imageStream, string imageName);
    public bool DeleteModelImage(string imageName);
  }
}