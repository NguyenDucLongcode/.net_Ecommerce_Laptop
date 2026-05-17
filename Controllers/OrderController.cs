using ComChienMaDui.Data;
using ComChienMaDui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ComChienMaDui.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {

        private readonly EcommerceLaptopContext _context;

        public OrderController(EcommerceLaptopContext context)
        {
            _context = context;
        }

        // Lấy UserId từ Claims của JWT và chuyển đổi sang số nguyên (int)
        private int? GetUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("UserId")?.Value
                ?? User.FindFirst("id")?.Value;

            if (int.TryParse(userIdString, out int userId))
            {
                return userId; // Trả về dạng int nếu Parse thành công
            }

            return null; // Trả về null nếu không tìm thấy hoặc sai format
        }

       

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == GetUserId())
                .ToListAsync();
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
