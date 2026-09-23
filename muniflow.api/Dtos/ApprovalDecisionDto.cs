using System.ComponentModel.DataAnnotations;

namespace muniflow.api.Dtos
{
    public class ApprovalDecisionDto
    {
        [Required]
        [MaxLength(100)]
        public string ApproverName { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Decision must be 'Approved' or 'Rejected'")]
        public string Decision { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Comments { get; set; }
    }
}