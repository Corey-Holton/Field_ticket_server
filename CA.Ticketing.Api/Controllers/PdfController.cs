using CA.Ticketing.Business.Services.Pdf;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private IPdfGeneratorService _pdfGeneratorService;

        public PdfController(IPdfGeneratorService pdfGeneratorService)
        {
            _pdfGeneratorService = pdfGeneratorService;
        }

        [HttpPost]
        [Route("test")]
        public async Task<IActionResult> TestPdf(IFormFile file)
        {
            var fileContent = "";
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            var pdfBytes = _pdfGeneratorService.GeneratePdf(fileContent);

            var stream = new MemoryStream(pdfBytes);

            return File(stream, "application/pdf", "TestPrint.pdf");
        }
    }
}
