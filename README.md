# End-to-End-Financial-Report-Generator
Submission for the Software Intern Technical Assessment - Kokoodi
Kokoodi Technical Assessment – End-to-End Financial Report Generator

Applicant: Nanji Emmanuella
Role: Software Intern – Microsoft Office Automation & Full-Stack Support
Submission Date: October 21, 2025

Submission Deadline: October 22, 2025

Overview

This project is a full-stack simulation of a Financial Report Generator for Microsoft Office automation.


## Part 1 – UI/UX & Modern Frontend (HTML/CSS/JavaScript)
To Design a clean, unique, and modern user interface optimized for a narrow, vertical Office Task Pane (approx 350px wide).

Features Implemented

Report Type (modern segmented toggle): P&L, Balance Sheet, or Cash Flow

Dynamic Reporting Year Dropdown: Automatically populates current year and previous 5 years

Client Name/ID Input Field

Primary Action Button: “Generate Report”

Bonus Feature: Loading Indicator & Success/Error Alerts (aria-live): Displays clear feedback during processing

## Part 2 – Agent Logic & Frontend-to-Backend Interface (JavaScript)
Function: prepareAgentRequest()

This function collects user inputs, validates all required fields, and returns a structured JSON payload ready for backend processing.

Validation Steps:

Ensures report type, year, and client name/ID are all filled.

BOnus Feature: Displays error messages with focus management for accessibility.

Bonus Feature: Adds a timestamp for auditing.

Example Output:

``{
  "reportType": "P&L",
  "reportYear": 2025,
  "clientName": "Acme Corp",
  "timestamp": "2024-10-22T17:30:00Z"
}   ``

### Backend API Design Explanation

Sample Service Endpoint:
POST https://api.kokoodi-finance.com/v1/report/generate

Backend Agent’s Role:
When the frontend sends the JSON payload containing the report type, reporting year, and client name,
the backend validates the data, checks for the client record in the database, and prepares necessary metadata for document creation.
After validation, it invokes the C#/OpenXML service (as seen in Part 3) to generate a formatted Word report (GeneratedReport.docx).

This separation ensures that heavy operations like Word file generation, database lookups, and secure data access occur on the trusted server-side, improving security, privacy, availability, data integrity, and scalability.

## Part 3 – Backend Research & Server-Side Implementation (C#/OpenXML)
Libraries & Namespaces Used
NuGet Package: DocumentFormat.OpenXml

Namespaces:
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

Implementation: CreateSimpleReport(string clientName, string reportType)
public static void CreateSimpleReport(string clientName, string reportType)
{
    string filePath = "GeneratedReport.docx";

    using (WordprocessingDocument wordDocument = 
        WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
    {
        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
        Body body = new Body();
        Paragraph para = new Paragraph(
            new Run(new Text($"Report: {reportType} for Client: {clientName}")));
        body.Append(para);
        mainPart.Document = new Document(body);
    }

    System.Console.WriteLine($"Report generated: {Path.GetFullPath(filePath)}");
}

Architectural Explanation

This C#/OpenXML process runs on the server because Word document generation requires:

File system access and .NET-specific libraries not available in browsers.

Secure handling of client data and metadata in a controlled backend environment.
Running server-side ensures compatibility, security, and reliability, while freeing the user’s Task Pane from heavy processing.

## Part 4 – Bonus Feature (Self-Directed)
Bonus Features Implemented

Accessibility Enhancements (ARIA, Roles):

Fully accessible aria-live alerts for screen-reader users, and logical tab order.

Improves inclusivity for screen reader and keyboard users.

Loading State + Feedback System(Shake function for report type):

Adds spinner and success/error alerts during report generation.

Dynamic Year Generation + Timestamped Payload:

Enables audit-ready data tracking on the backend.

Value

These enhancements reflect real-world production awareness — focusing on accessibility and efficiency.

