using System;
using System.Collections.Generic;

namespace ComChienMaDui.Models;

public partial class Specification
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? Cpu { get; set; }

    public string? Ram { get; set; }

    public string? Storage { get; set; }

    public string? Screen { get; set; }

    public string? Graphics { get; set; }

    public virtual Product Product { get; set; } = null!;
}
