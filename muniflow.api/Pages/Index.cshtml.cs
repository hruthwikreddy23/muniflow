using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using muniflow.api.Data;

namespace muniflow.api.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MuniFlowDbContext _context;

        public IndexModel(MuniFlowDbContext context)
        {
            _context = context;
        }

        public int TotalPOs { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal TotalApprovedAmount { get; set; }

        public async Task OnGetAsync()
        {
            TotalPOs = await _context.PurchaseOrders.CountAsync();
            PendingCount = await _context.PurchaseOrders.CountAsync(p => p.Status == "Pending");
            ApprovedCount = await _context.PurchaseOrders.CountAsync(p => p.Status == "Approved");
            RejectedCount = await _context.PurchaseOrders.CountAsync(p => p.Status == "Rejected");
            TotalApprovedAmount = await _context.PurchaseOrders
                .Where(p => p.Status == "Approved")
                .SumAsync(p => p.Amount);
        }
    }
}