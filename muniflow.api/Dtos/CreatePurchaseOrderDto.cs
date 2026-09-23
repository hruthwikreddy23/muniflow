using System.ComponentModel.DataAnnotations;

namespace muniflow.api.Dtos
{
    public class CreatePurchaseOrderDto
    {
        [Required]
        [MaxLength(200)]
        public string VendorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000000)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(100)]
        public string SubmittedBy { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }
    }
}