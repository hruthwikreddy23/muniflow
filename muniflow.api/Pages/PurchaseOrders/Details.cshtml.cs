using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using muniflow.api.Data;
using muniflow.api.Models;

namespace muniflow.api.Pages.PurchaseOrders
{
    public class DetailsModel : PageModel
    {
        private readonly MuniFlowDbContext _context;

        public DetailsModel(MuniFlowDbContext context)
        {
            _context = context;
        }

        public PurchaseOrder? PurchaseOrder { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task OnGetAsync(int id)
        {
            PurchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Department)
                .Include(po => po.Approvals)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);
        }

        public async Task<IActionResult> OnPostAsync(int id, string approverName, string decision, string? comments)
        {
            var po = await _context.PurchaseOrders
                .Include(p => p.Department)
                .Include(p => p.Approvals)
                .FirstOrDefaultAsync(p => p.PurchaseOrderId == id);

            if (po == null)
            {
                return NotFound();
            }

            if (po.Status != "Pending")
            {
                PurchaseOrder = po;
                return Page();
            }

            var approval = new Approval
            {
                ApproverName = approverName,
                Decision = decision,
                Comments = comments,
                DecisionDate = DateTime.UtcNow,
                PurchaseOrderId = po.PurchaseOrderId
            };

            _context.Approvals.Add(approval);

            po.Status = decision;
            po.ProcessedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToPage("/PurchaseOrders/Details", new { id = po.PurchaseOrderId });
        }
    }
}