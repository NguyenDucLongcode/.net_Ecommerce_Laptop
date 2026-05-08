using System;
using System.Collections.Generic;

namespace ComChienMaDui.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
