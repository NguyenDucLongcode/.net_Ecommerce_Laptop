using ComChienMaDui.Models;
using EcommerceLaptop.Models;
using Microsoft.EntityFrameworkCore;

namespace ComChienMaDui.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Payment> Payments { get; set; }
    }
}