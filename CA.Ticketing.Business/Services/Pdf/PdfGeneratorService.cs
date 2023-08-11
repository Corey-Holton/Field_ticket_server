using SelectPdf;

namespace CA.Ticketing.Business.Services.Pdf
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public byte[] GeneratePdf(string htmlInput)
        {
            var converter = new HtmlToPdf();

            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.PdfPageSize = PdfPageSize.Letter;
            converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;

            converter.Options.MarginLeft = 0;
            converter.Options.MarginRight = 0;
            converter.Options.MarginBottom = 0;
            converter.Options.MarginTop = 0;

            converter.Options.WebPageWidth = 0;
            converter.Options.WebPageHeight = 0;
            converter.Options.WebPageFixedSize = false;

            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.ShrinkOnly;
            var cardPdf = converter.ConvertHtmlString(htmlInput);
            var cardBytes = cardPdf.Save();
            cardPdf.Close();

            return cardBytes;
        }
    }
}
