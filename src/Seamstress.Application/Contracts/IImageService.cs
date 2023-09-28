using Microsoft.AspNetCore.Http;

namespace Seamstress.Application.Contracts
{
  public interface IImageService
  {
    public Task<string> UpdateImage(List<IFormFile> formFile, int itemId);
    public Task<string> SaveImage(IFormFile imageFile);
    public void DeleteImage(string imageName);
  }
}