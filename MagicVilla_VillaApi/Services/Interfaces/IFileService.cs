namespace MagicVilla_VillaApi.Services.Interfaces
{
    public interface IFileService
    {
        public Task<(string LocalPath, string Url)> UploudFileWithId<T>(IFormFile file, string baseUrl, T Id);
        public void Delete(string path);
    }
}
