using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using muniflow.api.Data;
using muniflow.api.Models;

namespace muniflow.api.Pages.PurchaseOrders
{
    public class CreateModel : PageModel
    {
        private readonly MuniFlowDbContext _context;

        public CreateModel(MuniFlowDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public POInput Input { get; set; } = new();

        public List<Department> Departments { get; set; } = new();

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields correctly.";
                return Page();
            }

            var department = await _context.Departments.FindAsync(Input.DepartmentId);
            if (department == null)
            {
                ErrorMessage = "Invalid department selected.";
                return Page();
            }

            var poNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

            var po = new PurchaseOrder
            {
                PONumber = poNumber,
                VendorName = Input.VendorName,
                Description = Input.Description,
                Amount = Input.Amount,
                Status = "Pending",
                SubmittedBy = Input.SubmittedBy,
                SubmittedAt = DateTime.UtcNow,
                DepartmentId = Input.DepartmentId
            };

            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();

            return RedirectToPage("/PurchaseOrders/Details", new { id = po.PurchaseOrderId });
        }

        public class POInput
        {
            public string VendorName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string SubmittedBy { get; set; } = string.Empty;
            public int DepartmentId { get; set; }
        }
    }
}