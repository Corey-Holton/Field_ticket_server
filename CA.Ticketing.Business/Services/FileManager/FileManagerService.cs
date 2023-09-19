using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Hosting;

namespace CA.Ticketing.Business.Services.FileManager
{
    public class FileManagerService : IFileManagerService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public FileManagerService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void SaveFile(byte[] bytes, string path, string fileName)
        {
            var folder = Path.Combine(_hostEnvironment.ContentRootPath, path);

            Directory.CreateDirectory(folder);
            
            var filePath = Path.Combine(folder, fileName);
            File.WriteAllBytes(filePath, bytes);
        }

        public byte[] GetFileBytes(string path, string fileName)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, path, fileName);
            
            if (!File.Exists(filePath))
            {
                return new byte[] { 0 };
            }

            return File.ReadAllBytes(filePath);
        }

        public void DeleteFile(string path, string fileName)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, path, fileName);

            if (!File.Exists(filePath))
            {
                return;
            }

            File.Delete(filePath);
        }
    }
}
