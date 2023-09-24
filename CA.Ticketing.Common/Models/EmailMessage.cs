namespace CA.Ticketing.Common.Models
{
    public class EmailMessage
    {
        public string Title { get; set; }

        public string Subject { get; set; }

        public List<MessageParagraph> Paragraphs { get; set; }

        public EmailMessage(string title, string subject, IEnumerable<MessageParagraph> paragraphs)
        {
            Title = title;
            Subject = subject;
            Paragraphs = paragraphs.ToList();
        }

        public void ApplyParameters((int order, object parameter)[] parameters)
        {
            foreach (var parameter in parameters)
            {
                var paragraph = Paragraphs.SingleOrDefault(x => x.Order == parameter.order);

                if (paragraph == null)
                {
                    continue;
                }

                paragraph.ApplyParameters(parameter.parameter);
            }
        }
    }

    public class MessageParagraph
    {
        public int Order { get; set; }

        public string Text { get; set; }

        public string ActionUrl { get; set; }

        public ParagraphTypes ParagraphType { get; set; }

        public void ApplyParameters(object parameter)
        {
            if (ParagraphType == ParagraphTypes.Action)
            {
                ActionUrl = parameter as string ?? string.Empty;
            }
            else
            {
                var stringParams = parameter as string[] ?? Array.Empty<string>();
                Text = string.Format(Text, stringParams);
            }
        }
    }

    public enum ParagraphTypes
    {
        Text = 0,
        AccessorialText = 1,
        Action = 2,
        Label = 3
    }
}
