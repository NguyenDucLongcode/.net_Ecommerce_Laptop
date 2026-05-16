using System;
using System.Collections.Generic;

namespace ComChienMaDui.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal OriginalPrice { get; set; }

    public int? DiscountPercent { get; set; }

    public int? Stock { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Specification> Specifications { get; set; } = new List<Specification>();

    // Add this property if it does not exist
    public string Image { get; set; } = null!;
}
