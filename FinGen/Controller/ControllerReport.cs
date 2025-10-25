using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace FinGen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateReport([FromBody] ReportRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ClientName) || string.IsNullOrWhiteSpace(request.ReportType))
                return BadRequest(new { message = "Client name and report type are required." });

            string fileName = $"Report_{request.ClientName}_{request.ReportType}.docx";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            // Create the Word document using OpenXML
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                Body body = new Body();
                Paragraph para = new Paragraph(new Run(new Text($"Report: {request.ReportType} for Client: {request.ClientName} ({request.ReportYear})")));
                body.Append(para);
                mainPart.Document = new Document(body);
            }

            return Ok(new
            {
                message = $"Report for {request.ClientName} ({request.ReportType}) successfully generated.",
                filePath
            });
        }
    }

    // DTO (Data Transfer Object)
    public class ReportRequest
    {
        public string ClientName { get; set; }
        public string ReportType { get; set; }
        public string ReportYear { get; set; }
    }
}
