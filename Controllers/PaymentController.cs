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

            // Đưa toàn bộ Danh sách chi tiết Order vào DB
            _context.OrderDetails.AddRange(orderDetails);

                // 2. XÓA TẤT CẢ SẢN PHẨM Ở GIỎ HÀNG THUỘC USER NÀY
                _context.Carts.RemoveRange(carts);
                _context.SaveChanges();

                // 3. Gửi thông báo thành công thông qua TempData
                TempData["SuccessMessage"] = $"Đã đặt hàng thành công! Đơn hàng của {FullName} sẽ được giao đến {Address}.";
            

                // 4. Chuyển hướng người dùng quay lại trang Giỏ hàng
                return RedirectToAction("Index", "Cart");
            }
        // DANH SÁCH ĐƠN HÀNG
    public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders.ToListAsync();

            return View(orders);
        }

        // CHI TIẾT ĐƠN HÀNG
        public async Task<IActionResult> OrderDetail(int id)
        {
            var details = await _context.OrderDetails
                .Include(x => x.Product)
                .Where(x => x.OrderId == id)
                .ToListAsync();

            return View(details);
        }
    }       
    
}