namespace CA.Ticketing.Business.Services.FileManager
{
    public interface IFileManagerService
    {
        void SaveFile(byte[] bytes, string path, string fileName);

        byte[] GetFileBytes(string path, string fileName);

        void DeleteFile(string path, string fileName);
    }
}
