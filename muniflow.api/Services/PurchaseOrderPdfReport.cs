using muniflow.api.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace muniflow.api.Services
{
    public class PurchaseOrderPdfReport : IDocument
    {
        private readonly PurchaseOrder _po;
        private readonly string _departmentName;

        public PurchaseOrderPdfReport(PurchaseOrder po, string departmentName)
        {
            _po = po;
            _departmentName = departmentName;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("MUNIFLOW").FontSize(24).Bold().FontColor(Colors.Blue.Darken3);
                        col.Item().Text("Municipal Purchase Order System").FontSize(10).Italic().FontColor(Colors.Grey.Darken2);
                    });

                    row.ConstantItem(200).Column(col =>
                    {
                        col.Item().AlignRight().Text("PURCHASE ORDER").FontSize(16).Bold();
                        col.Item().AlignRight().Text($"#{_po.PONumber}").FontSize(12).FontColor(Colors.Grey.Darken2);
                    });
                });

                column.Item().PaddingVertical(10).LineHorizontal(2).LineColor(Colors.Blue.Darken3);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(15);

                // PO Status Banner
                var statusColor = _po.Status switch
                {
                    "Approved" => Colors.Green.Lighten3,
                    "Rejected" => Colors.Red.Lighten3,
                    _ => Colors.Yellow.Lighten3
                };
                var statusTextColor = _po.Status switch
                {
                    "Approved" => Colors.Green.Darken3,
                    "Rejected" => Colors.Red.Darken3,
                    _ => Colors.Orange.Darken3
                };

                column.Item().Background(statusColor).Padding(10).Row(row =>
                {
                    row.RelativeItem().Text("STATUS:").Bold();
                    row.RelativeItem().AlignRight().Text(_po.Status.ToUpper()).Bold().FontColor(statusTextColor);
                });

                // PO Details Section
                column.Item().Text("Purchase Order Details").FontSize(14).Bold().FontColor(Colors.Blue.Darken3);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(2);
                    });

                    table.Cell().Element(CellStyle).Text("PO Number:").Bold();
                    table.Cell().Element(CellStyle).Text(_po.PONumber);

                    table.Cell().Element(CellStyle).Text("Vendor:").Bold();
                    table.Cell().Element(CellStyle).Text(_po.VendorName);

                    table.Cell().Element(CellStyle).Text("Department:").Bold();
                    table.Cell().Element(CellStyle).Text(_departmentName);

                    table.Cell().Element(CellStyle).Text("Description:").Bold();
                    table.Cell().Element(CellStyle).Text(_po.Description);

                    table.Cell().Element(CellStyle).Text("Amount:").Bold();
                    table.Cell().Element(CellStyle).Text($"${_po.Amount:N2}").Bold().FontColor(Colors.Blue.Darken3);

                    table.Cell().Element(CellStyle).Text("Submitted By:").Bold();
                    table.Cell().Element(CellStyle).Text(_po.SubmittedBy);

                    table.Cell().Element(CellStyle).Text("Submitted At:").Bold();
                    table.Cell().Element(CellStyle).Text(_po.SubmittedAt.ToString("MMMM dd, yyyy HH:mm"));

                    if (_po.ProcessedAt.HasValue)
                    {
                        table.Cell().Element(CellStyle).Text("Processed At:").Bold();
                        table.Cell().Element(CellStyle).Text(_po.ProcessedAt.Value.ToString("MMMM dd, yyyy HH:mm"));
                    }

                    static IContainer CellStyle(IContainer container) =>
                        container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                });

                // Approval History Section
                if (_po.Approvals != null && _po.Approvals.Any())
                {
                    column.Item().PaddingTop(20).Text("Approval History").FontSize(14).Bold().FontColor(Colors.Blue.Darken3);

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(3);
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Approver").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Decision").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Date").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Comments").FontColor(Colors.White).Bold();
                        });

                        // Rows
                        foreach (var approval in _po.Approvals.OrderBy(a => a.DecisionDate))
                        {
                            table.Cell().Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(approval.ApproverName);
                            table.Cell().Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(approval.Decision);
                            table.Cell().Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(approval.DecisionDate.ToString("MM/dd/yyyy HH:mm"));
                            table.Cell().Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(approval.Comments ?? "-");
                        }
                    });
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text($"Generated on {DateTime.UtcNow:MMMM dd, yyyy HH:mm} UTC").FontSize(8).FontColor(Colors.Grey.Darken1);
                    row.RelativeItem().AlignRight().Text("MuniFlow v1.0 - Confidential").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }
    }
}