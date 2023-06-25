using CA.Ticketing.Business.Services.Notifications.Renderers;
using CA.Ticketing.Common.Models;
using CA.Ticketing.Common.Setup;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CA.Ticketing.Business.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IRazorViewToStringRenderer _razorRenderer;

        private readonly EmailSettings _emailSettings;

        private const string _emailTemplatePath = "/Views/Email/_EmailTemplate.cshtml";

        public NotificationService(IRazorViewToStringRenderer razorRenderer, IOptions<EmailSettings> emailSettings)
        {
            _razorRenderer = razorRenderer;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmail(string emailAddress, EmailMessage emailMessage, IEnumerable<(Stream AttachmentStream, string Name)>? attachments = null)
        {
            var emailBody = await _razorRenderer.RenderViewToStringAsync(_emailTemplatePath, emailMessage);
            await SendEmail(emailAddress, emailAddress, emailMessage.Subject, emailBody, attachments);
        }

        private async Task SendEmail(string recipientEmail, string recipientDisplayName, string subject, string messageHtml, IEnumerable<(Stream AttachmentStream, string Name)>? attachments = null)
        {
            var smtpClient = new SmtpClient(_emailSettings.SmtpUrl, _emailSettings.SmtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderDisplayName),
                Subject = subject,
                Body = messageHtml,
                IsBodyHtml = true
            };

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(attachment.AttachmentStream, attachment.Name));
                }
            }

            mailMessage.To.Add(new MailAddress(recipientEmail, recipientDisplayName));

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
