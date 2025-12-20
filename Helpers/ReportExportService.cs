using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ERP.Helpers
{
    public interface IReportExportService
    {
        Task<string> ExportInvoicesAsync(IEnumerable<DTOs.Invoices.InvoiceDTO> invoices, string format, DTOs.Invoices.InvoiceBrandingDTO? branding = null);
    }

    public class ReportExportService : IReportExportService
    {
        private readonly string _basePath;

        public ReportExportService(IConfiguration config)
        {
            _basePath = config["ExportPath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "Exports");
        }

        public async Task<string> ExportInvoicesAsync(IEnumerable<DTOs.Invoices.InvoiceDTO> invoices, string format, DTOs.Invoices.InvoiceBrandingDTO? branding = null)
        {
            var safeFormat = format?.ToLowerInvariant();
            if (safeFormat != "pdf" && safeFormat != "excel")
                throw new InvalidOperationException("Unsupported export format");

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            var fileName = $"invoices_{DateTime.UtcNow:yyyyMMddHHmmssfff}.{(safeFormat == "pdf" ? "html" : "csv")}";
            var fullPath = Path.Combine(_basePath, fileName);

            if (safeFormat == "excel")
            {
                var csv = BuildCsv(invoices);
                await File.WriteAllTextAsync(fullPath, csv, Encoding.UTF8);
            }
            else
            {
                var html = BuildHtml(invoices, branding);
                await File.WriteAllTextAsync(fullPath, html, Encoding.UTF8);
            }

            return fullPath;
        }

        private string BuildCsv(IEnumerable<DTOs.Invoices.InvoiceDTO> invoices)
        {
            var sb = new StringBuilder();
            sb.AppendLine("InvoiceNumber,Date,DueDate,Type,Status,Total,PaidAmount,SupplierId,ClientId,ProjectId");
            foreach (var inv in invoices)
            {
                sb.AppendLine($"\"{inv.InvoiceNumber}\",{inv.InvoiceDate},{inv.DueDate},{inv.Type},{inv.Status},{inv.Total},{inv.PaidAmount},{inv.SupplierId},{inv.ClientId},{inv.ProjectId}");
            }
            return sb.ToString();
        }

        private string BuildHtml(IEnumerable<DTOs.Invoices.InvoiceDTO> invoices, DTOs.Invoices.InvoiceBrandingDTO? branding)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset='utf-8'><style>body{font-family:Arial;}table{width:100%;border-collapse:collapse;}th,td{border:1px solid #ccc;padding:8px;text-align:left;}th{background:#f5f5f5;}</style></head><body>");
            if (branding != null)
            {
                sb.AppendLine("<div style='display:flex;align-items:center;gap:12px;margin-bottom:16px;'>");
                if (!string.IsNullOrWhiteSpace(branding.LogoUrl))
                {
                    sb.AppendLine($"<img src='{branding.LogoUrl}' alt='Logo' style='height:50px;'>");
                }
                if (!string.IsNullOrWhiteSpace(branding.Header))
                    sb.AppendLine($"<div><h2 style='margin:0'>{branding.Header}</h2></div>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("<table><thead><tr><th>Invoice #</th><th>Date</th><th>Due</th><th>Type</th><th>Status</th><th>Total</th><th>Paid</th><th>Supplier</th><th>Client</th><th>Project</th></tr></thead><tbody>");
            foreach (var inv in invoices)
            {
                sb.AppendLine($"<tr><td>{inv.InvoiceNumber}</td><td>{inv.InvoiceDate}</td><td>{inv.DueDate}</td><td>{inv.Type}</td><td>{inv.Status}</td><td>{inv.Total}</td><td>{inv.PaidAmount}</td><td>{inv.SupplierId}</td><td>{inv.ClientId}</td><td>{inv.ProjectId}</td></tr>");
            }
            sb.AppendLine("</tbody></table>");
            if (branding != null && !string.IsNullOrWhiteSpace(branding.Footer))
            {
                sb.AppendLine($"<p style='margin-top:24px;font-size:12px;color:#555'>{branding.Footer}</p>");
            }
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }
    }
}
