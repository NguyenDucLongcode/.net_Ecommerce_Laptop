using ComChienMaDui.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComChienMaDui.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = "Pending";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public string? TransactionId { get; set; }

        // Navigation
        public virtual Order? Order { get; set; }
    }
}