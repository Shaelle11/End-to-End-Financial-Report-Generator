using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

namespace FinGen.Controller
{
    [ApiController]
    [Route("api/report")]
    public class ControllerReport : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateReport([FromBody] ReportRequest request)
        {
            if (string.IsNullOrEmpty(request.ClientName) || string.IsNullOrEmpty(request.ReportType))
                return BadRequest("Missing required fields.");

            using (var ms = new MemoryStream())
            {
                using (var wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    var mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    // Title
                    var titleParagraph = new Paragraph(new Run(new Text("Financial Report Generator")));
                    titleParagraph.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new SpacingBetweenLines() { After = "200" }
                    );
                    titleParagraph.ParagraphProperties.ParagraphStyleId = new ParagraphStyleId() { Val = "Title" };
                    body.AppendChild(titleParagraph);

                    // Report details
                    body.AppendChild(CreateStyledParagraph($"Report Type: {request.ReportType}"));
                    body.AppendChild(CreateStyledParagraph($"Reporting Year: {request.ReportYear}"));
                    body.AppendChild(CreateStyledParagraph($"Client Name/ID: {request.ClientName}"));

                    // Footer
                    body.AppendChild(CreateStyledParagraph($"Generated on: {System.DateTime.Now.ToString("f")}"));

                    mainPart.Document.Save();
                }

                ms.Seek(0, SeekOrigin.Begin);
                return File(ms.ToArray(),
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    $"FinancialReport_{request.ClientName}_{request.ReportType}_{request.ReportYear}.docx");
            }
        }

        private Paragraph CreateStyledParagraph(string text)
        {
            var p = new Paragraph();
            var run = new Run();
            run.Append(new Text(text));
            p.Append(run);
            p.ParagraphProperties = new ParagraphProperties(
                new SpacingBetweenLines() { After = "120" },
                new Justification() { Val = JustificationValues.Left }
            );
            return p;
        }

        public class ReportRequest
        {
            public string ClientName { get; set; } = string.Empty;
            public string ReportType { get; set; } = string.Empty;
            public string ReportYear { get; set; } = string.Empty;
        }
    }
}
