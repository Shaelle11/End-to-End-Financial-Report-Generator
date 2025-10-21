using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

public class ReportGenerator
{
    public static void CreateSimpleReport(string clientName, string reportType)
    {
        string filePath = "GeneratedReport.docx";

        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
        {
            // Add a main document part. 
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            // Create the document structure and add some text.

            Body body = new Body();
            Paragraph para = new Paragraph(new Run(new Text($"Report: {reportType} for Client: {clientName}")));
            body.Append(para);
            mainPart.Document = new Document(body);
        }
        // Optionally, confirm the file was created
        System.Console.WriteLine($"Report generated: {Path.GetFullPath(filePath)}");
    }
}
