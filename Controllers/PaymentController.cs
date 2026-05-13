using ComChienMaDui.Data;
using ComChienMaDui.Models;
using EcommerceLaptop.Data;
using EcommerceLaptop.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComChienMaDui.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang checkout
        public IActionResult Checkout(int orderId)
        {
            var order = _context.Orders
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Thanh toán COD
        [HttpPost]
        public IActionResult ProcessCOD(int orderId)
        {
            var order = _context.Orders
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            Payment payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = "COD",
                PaymentStatus = "Pending",
                Amount = order.TotalAmount,
                PaymentDate = DateTime.Now
            };

            order.Status = "Processing";

            _context.Payments.Add(payment);

            _context.SaveChanges();

            return RedirectToAction("Success");
        }

        // Thành công
        public IActionResult Success()
        {
            return View();
        }
    }
}