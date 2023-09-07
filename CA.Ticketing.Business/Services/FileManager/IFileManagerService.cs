namespace CA.Ticketing.Business.Services.FileManager
{
    public interface IFileManagerService
    {
        void SaveTicketFile(byte[] bytes, string fileName);

        byte[] GetTicketBytes(string fileName);

        void DeleteTicket(string fileName);
    }
}
