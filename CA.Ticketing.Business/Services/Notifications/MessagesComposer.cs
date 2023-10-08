using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Common.Models;
using Microsoft.AspNetCore.Hosting;

namespace CA.Ticketing.Business.Services.Notifications
{
    public class MessagesComposer
    {
        private const string _emailTemplatesFile = "emailTemplates.json";

        private readonly List<EmailTemplate> _emailTemplates = new();

        public MessagesComposer(IWebHostEnvironment hostEnvironment)
        {
            var filePath = Path.Combine(hostEnvironment.ContentRootPath, "Resources", _emailTemplatesFile);
            if (!File.Exists(filePath))
            {
                return;
            }

            var fileContent = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            var emailTemplates = fileContent.FromJson<List<EmailTemplate>>() ?? new List<EmailTemplate>();
            _emailTemplates.AddRange(emailTemplates);
        }

        public EmailMessage GetEmailComposed(string emailMessageType, params (int order, object parameter)[] parameters)
        {
            var emailTemplate = _emailTemplates.Single(x => x.TemplateKey == emailMessageType);

            var paragraphs = emailTemplate.Paragraphs.Select(x => new MessageParagraph
            {
                Order = x.Order,
                ParagraphType = (ParagraphTypes)x.ParagraphType,
                Text = x.Text
            })
            .ToList();

            foreach (var parameter in parameters)
            {
                var paragraph = paragraphs
                    .SingleOrDefault(x => x.Order == parameter.order);
                if (paragraph == null)
                {
                    continue;
                }

                paragraph.ApplyParameters(parameter.parameter);
            }

            var emailMessage = new EmailMessage(emailTemplate.Title, emailTemplate.Subject, paragraphs);
            return emailMessage;
        }
    }
}
