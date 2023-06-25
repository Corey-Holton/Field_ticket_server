using CA.Ticketing.Common.Models;

namespace CA.Ticketing.Business.Services.Notifications
{
    public interface INotificationService
    {
        Task SendEmail(string emailAddress, EmailMessage emailMessage, IEnumerable<(Stream AttachmentStream, string Name)>? attachments = null);
    }
}
