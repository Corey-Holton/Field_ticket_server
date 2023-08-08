using SelectPdf;

namespace CA.Ticketing.Business.Services.Pdf
{
    public interface IPdfGeneratorService
    {
        byte[] GeneratePdf(string htmlInput);
    }
}
