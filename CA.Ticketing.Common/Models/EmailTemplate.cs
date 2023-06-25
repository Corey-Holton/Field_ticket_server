namespace CA.Ticketing.Common.Models
{
    public class EmailTemplate
    {
        public string TemplateKey { get; set; }

        public string Title { get; set; }

        public string Subject { get; set; }

        public IEnumerable<EmailTemplateParagraph> Paragraphs { get; set; }
    }

    public class EmailTemplateParagraph
    {
        public int Order { get; set; }

        public int ParagraphType { get; set; }

        public string Text { get; set; }
    }
}
