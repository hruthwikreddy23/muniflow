using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using muniflow.api.Data;
using muniflow.api.Dtos;
using muniflow.api.Models;
using muniflow.api.Services;
using QuestPDF.Fluent;


namespace muniflow.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly MuniFlowDbContext _context;

        public PurchaseOrdersController(MuniFlowDbContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderResponseDto>>> GetPurchaseOrders()
        {
            var pos = await _context.PurchaseOrders
                .Include(po => po.Department)
                .OrderByDescending(po => po.SubmittedAt)
                .Select(po => new PurchaseOrderResponseDto
                {
                    PurchaseOrderId = po.PurchaseOrderId,
                    PONumber = po.PONumber,
                    VendorName = po.VendorName,
                    Description = po.Description,
                    Amount = po.Amount,
                    Status = po.Status,
                    SubmittedBy = po.SubmittedBy,
                    SubmittedAt = po.SubmittedAt,
                    ProcessedAt = po.ProcessedAt,
                    DepartmentId = po.DepartmentId,
                    DepartmentName = po.Department != null ? po.Department.Name : string.Empty
                })
                .ToListAsync();

            return Ok(pos);
        }

        // GET: api/PurchaseOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> GetPurchaseOrder(int id)
        {
            var po = await _context.PurchaseOrders
                .Include(po => po.Department)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);

            if (po == null)
            {
                return NotFound(new { message = $"Purchase Order with ID {id} not found." });
            }

            var response = new PurchaseOrderResponseDto
            {
                PurchaseOrderId = po.PurchaseOrderId,
                PONumber = po.PONumber,
                VendorName = po.VendorName,
                Description = po.Description,
                Amount = po.Amount,
                Status = po.Status,
                SubmittedBy = po.SubmittedBy,
                SubmittedAt = po.SubmittedAt,
                ProcessedAt = po.ProcessedAt,
                DepartmentId = po.DepartmentId,
                DepartmentName = po.Department != null ? po.Department.Name : string.Empty
            };

            return Ok(response);
        }

        // POST: api/PurchaseOrders
        [HttpPost]
        public async Task<ActionResult<PurchaseOrderResponseDto>> CreatePurchaseOrder(CreatePurchaseOrderDto dto)
        {
            // Validate department exists
            var department = await _context.Departments.FindAsync(dto.DepartmentId);
            if (department == null)
            {
                return BadRequest(new { message = $"Department with ID {dto.DepartmentId} does not exist." });
            }

            // Generate unique PO number
            var poNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

            var po = new PurchaseOrder
            {
                PONumber = poNumber,
                VendorName = dto.VendorName,
                Description = dto.Description,
                Amount = dto.Amount,
                Status = "Pending",
                SubmittedBy = dto.SubmittedBy,
                SubmittedAt = DateTime.UtcNow,
                DepartmentId = dto.DepartmentId
            };

            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();

            var response = new PurchaseOrderResponseDto
            {
                PurchaseOrderId = po.PurchaseOrderId,
                PONumber = po.PONumber,
                VendorName = po.VendorName,
                Description = po.Description,
                Amount = po.Amount,
                Status = po.Status,
                SubmittedBy = po.SubmittedBy,
                SubmittedAt = po.SubmittedAt,
                ProcessedAt = po.ProcessedAt,
                DepartmentId = po.DepartmentId,
                DepartmentName = department.Name
            };

            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = po.PurchaseOrderId }, response);
        }

        // POST: api/PurchaseOrders/5/approve
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> ApproveOrRejectPO(int id, ApprovalDecisionDto dto)
        {
            var po = await _context.PurchaseOrders
                .Include(po => po.Department)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);

            if (po == null)
            {
                return NotFound(new { message = $"Purchase Order with ID {id} not found." });
            }

            // Business rule: only Pending POs can be approved/rejected
            if (po.Status != "Pending")
            {
                return BadRequest(new { message = $"Cannot approve/reject a PO with status '{po.Status}'. Only Pending POs can be processed." });
            }

            // Create the approval audit record
            var approval = new Approval
            {
                ApproverName = dto.ApproverName,
                Decision = dto.Decision,
                Comments = dto.Comments,
                DecisionDate = DateTime.UtcNow,
                PurchaseOrderId = po.PurchaseOrderId
            };

            _context.Approvals.Add(approval);

            // Update the PO status
            po.Status = dto.Decision;
            po.ProcessedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new PurchaseOrderResponseDto
            {
                PurchaseOrderId = po.PurchaseOrderId,
                PONumber = po.PONumber,
                VendorName = po.VendorName,
                Description = po.Description,
                Amount = po.Amount,
                Status = po.Status,
                SubmittedBy = po.SubmittedBy,
                SubmittedAt = po.SubmittedAt,
                ProcessedAt = po.ProcessedAt,
                DepartmentId = po.DepartmentId,
                DepartmentName = po.Department != null ? po.Department.Name : string.Empty
            };

            return Ok(response);
        }

        // GET: api/PurchaseOrders/5/pdf
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadPurchaseOrderPdf(int id)
        {
            var po = await _context.PurchaseOrders
                .Include(po => po.Department)
                .Include(po => po.Approvals)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);

            if (po == null)
            {
                return NotFound(new { message = $"Purchase Order with ID {id} not found." });
            }

            var departmentName = po.Department?.Name ?? "Unknown";
            var report = new PurchaseOrderPdfReport(po, departmentName);
            var pdfBytes = report.GeneratePdf();

            var fileName = $"{po.PONumber}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}