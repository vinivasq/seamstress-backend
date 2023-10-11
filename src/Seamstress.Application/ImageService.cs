using Microsoft.AspNetCore.Http;
using Seamstress.Application.Contracts;

namespace Seamstress.Application
{
  public class ImageService : IImageService
  {
    private readonly IItemService _itemService;
    private readonly IAzureBlobService _azureBlobService;

    public ImageService(
                          IItemService itemService,
                          IAzureBlobService azureBlobService
                        )
    {
      this._azureBlobService = azureBlobService;
      this._itemService = itemService;
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
            if (DeleteImage(imageName))
              imageNames.Remove(imageName);
            else
            {
              throw new Exception($"Não foi possível remover a imagem {imageName}");
            }
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

      return await this._azureBlobService.UploadModelImageAsync(imageFile, imageName);
    }

    public bool DeleteImage(string imageName)
    {
      return this._azureBlobService.DeleteModelImage(imageName);
    }
  }
}