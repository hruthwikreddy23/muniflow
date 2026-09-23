using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using muniflow.api.Data;
using muniflow.api.Models;

namespace muniflow.api.Pages.PurchaseOrders
{
    public class IndexModel : PageModel
    {
        private readonly MuniFlowDbContext _context;

        public IndexModel(MuniFlowDbContext context)
        {
            _context = context;
        }

        public List<PurchaseOrder> PurchaseOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            PurchaseOrders = await _context.PurchaseOrders
                .Include(po => po.Department)
                .OrderByDescending(po => po.SubmittedAt)
                .ToListAsync();
        }
    }
}