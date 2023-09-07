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

        public void SaveTicketFile(byte[] bytes, string fileName)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, FilePaths.Tickets, fileName);
            File.WriteAllBytes(filePath, bytes);
        }

        public byte[] GetTicketBytes(string fileName)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, FilePaths.Tickets, fileName);
            
            if (!File.Exists(filePath))
            {
                return new byte[] { 0 };
            }

            return File.ReadAllBytes(filePath);
        }

        public void DeleteTicket(string fileName)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, FilePaths.Tickets, fileName);

            if (!File.Exists(filePath))
            {
                return;
            }

            File.Delete(filePath);
        }
    }
}
