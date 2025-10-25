using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace FinancialReportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult Generate([FromBody] ReportRequest request)
        {
            if (string.IsNullOrEmpty(request.ClientName) || string.IsNullOrEmpty(request.ReportType))
                return BadRequest("Invalid data.");

            string filePath = Path.Combine("Reports", $"{request.ClientName}_{request.ReportType}.docx");
            Directory.CreateDirectory("Reports");

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document(new Body(
                    new Paragraph(new Run(new Text($"Financial Report"))),
                    new Paragraph(new Run(new Text($"Client: {request.ClientName}"))),
                    new Paragraph(new Run(new Text($"Type: {request.ReportType}"))),
                    new Paragraph(new Run(new Text($"Year: {request.ReportYear}"))),
                    new Paragraph(new Run(new Text($"Generated at: {System.DateTime.Now}")))
                ));
            }

            // Return file as download
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", Path.GetFileName(filePath));
        }
    }

    public class ReportRequest
    {
        public string ClientName { get; set; }
        public string ReportType { get; set; }
        public string ReportYear { get; set; }
    }
}
