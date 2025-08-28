using Azure.Core;
using MagicVilla_VillaApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Buffers.Text;

namespace MagicVilla_VillaApi.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(string LocalPath, string Url)> UploudFileWithId<T>(IFormFile file,string baseUrl, T Id)
        {
            if (file == null) return (null, null);
            var name = Id + Path.GetExtension(file.FileName);
            var localPath = Path.Combine(_webHostEnvironment.WebRootPath, "Img", name);

            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
            if (!Directory.Exists(_webHostEnvironment.WebRootPath+"Img"))
            {
                Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "Img");
            }
            using (var fileStream = new FileStream(localPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var Url = $"{baseUrl}/Img/{name}";
            return (localPath,Url);
        }

        public void Delete(string path) {
        
            if(string.IsNullOrWhiteSpace( path)) return;
            if (File.Exists(path))
            {
                  File.Delete(path);
            }
        }

    }
}
