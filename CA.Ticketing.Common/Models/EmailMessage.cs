namespace CA.Ticketing.Common.Models
{
    public interface IMessageListItem
    {
        string Description { get; set; }

        double Quantity { get; set; }

        double Price { get; set; }
    }

    public class MessageListItem : IMessageListItem
    {
        public string Description { get; set; }

        public double Quantity { get; set; }

        public double Price { get; set; }

        public MessageListItem(string description, double quantity, double price)
        {
            Description = description;
            Quantity = quantity;
            Price = price;
        }
    }

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

        public IEnumerable<IMessageListItem> ListItems { get; set; }

        public ParagraphTypes ParagraphType { get; set; }

        public void ApplyParameters(object parameter)
        {
            if (ParagraphType == ParagraphTypes.Action)
            {
                ActionUrl = parameter as string ?? string.Empty;
            }
            else if (ParagraphType == ParagraphTypes.List)
            {
                ListItems = parameter as IEnumerable<IMessageListItem> ?? new List<IMessageListItem>();
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
        Label = 3,
        List = 4,
        GiftMessage = 5
    }

    public enum MessageListItemType
    {
        Simple,
        Priced
    }
}
