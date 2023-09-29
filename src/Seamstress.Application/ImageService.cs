using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Seamstress.Application.Contracts;

namespace Seamstress.Application
{
  public class ImageService : IImageService
  {

    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IItemService _itemService;

    public ImageService(IWebHostEnvironment webHostEnvironment, IItemService itemService)
    {
      this._itemService = itemService;
      this._webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> UpdateImage(List<IFormFile> formFiles, int itemId)
    {
      try
      {
        if (formFiles.Count == 0) throw new Exception("Não foi possível realizar o upload da imagem. Arquivos não encontrados");
        List<string> lstImages = new();

        if (itemId > 0)
        {
          var item = await _itemService.GetItemByIdAsync(itemId) ?? throw new Exception("Não foi possível realizar o upload da imagem. Modelo não encontrado");

          if (item.ImageURL == null)
          {
            formFiles.ForEach(file =>
            {
              lstImages.Add(SaveImage(file).Result);
            });

            return string.Join(';', lstImages);
          }

          List<string> imageNames = item.ImageURL.Split(';').ToList();
          List<string> formFilesNames = formFiles.Select(x => x.FileName).ToList();

          var imagesToAdd = formFilesNames.Except(imageNames).ToList();
          var imagesToRemove = imageNames.Except(formFilesNames).ToList();

          imagesToAdd.ForEach(imageName =>
          {
            IFormFile imageToAdd = formFiles.Where(x => x.FileName == imageName).First();
            imageNames.Add(SaveImage(imageToAdd).Result);
          });

          imagesToRemove.ForEach(imageName =>
          {
            DeleteImage(imageName);
            imageNames.Remove(imageName);
          });

          return string.Join(';', imageNames);
        }

        formFiles.ForEach(file =>
        {
          lstImages.Add(SaveImage(file).Result);
        });

        return string.Join(';', lstImages);
      }
      catch (Exception ex)
      {

        throw new Exception(ex.Message);
      }
    }

    public async Task<string> SaveImage(IFormFile imageFile)
    {
      string imageName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
      string imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources/Images", imageName);

      using (var fileStream = new FileStream(imagePath, FileMode.Create))
      {
        await imageFile.CopyToAsync(fileStream);
      }

      return imageName;
    }

    public void DeleteImage(string imageName)
    {
      string imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources/Images", imageName);
      if (File.Exists(imagePath)) File.Delete(imagePath);
    }
  }
}