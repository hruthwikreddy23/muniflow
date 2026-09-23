using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace muniflow.api.Models
{
    public class Approval
    {
        [Key]
        public int ApprovalId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ApproverName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Decision { get; set; } = string.Empty;
        // Possible values: Approved, Rejected

        [MaxLength(500)]
        public string? Comments { get; set; }

        public DateTime DecisionDate { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int PurchaseOrderId { get; set; }

        // Navigation property
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}