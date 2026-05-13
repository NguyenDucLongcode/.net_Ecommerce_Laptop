using ComChienMaDui.Models;
using System.ComponentModel.DataAnnotations;

namespace ComChienMaDui.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        // Navigation
        public virtual ICollection<Payment>? Payments { get; set; }
    }
}