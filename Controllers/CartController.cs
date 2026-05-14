using ComChienMaDui.Data;
using ComChienMaDui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; // Bắt buộc thêm using này ở đầu file để gọi Include()

namespace ComChienMaDui.Controllers
{
    [Authorize] // Bắt buộc người dùng phải có token hợp lệ để thao tác với giỏ hàng
    public class CartController : Controller
    {
        private readonly EcommerceLaptopContext _context;

        public CartController(EcommerceLaptopContext context)
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

        // Lấy giỏ hàng từ Database dựa trên UserId (kiểu int)
        private List<Cart> GetCartItems(int userId)
        {
            // Thêm .Include(c => c.Product) để truy vấn kèm theo thông tin của bảng Product
            return _context.Carts
                           .Include(c => c.Product) 
                           .Where(c => c.UserId == userId)
                           .ToList();
        }

        // GIAO DIỆN CHÍNH CỦA GIỎ HÀNG
        public IActionResult Index()
        {
            var userId = GetUserId();
            if (userId == null) // Kiểm tra xem user id có hợp lệ không
            {
                return Unauthorized("Không thể xác thực danh tính người dùng.");
            }

            var cart = GetCartItems(userId.Value);
            return View(cart);
        }

        // XỬ LÝ THÊM VÀO GIỎ HÀNG
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized("Không thể xác thực danh tính người dùng.");
            }

            var product = _context.Products.Find(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm");

            // Kiểm tra xem Database đã có item này trong giỏ của user chưa (userId.Value)
            var cartItem = _context.Carts.FirstOrDefault(p => p.ProductId == id && p.UserId == userId.Value);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity; // Nếu có rồi thì cộng dồn số lượng
                _context.Carts.Update(cartItem);
            }
            else
            {
                // Thêm sản phẩm mới vào DB
                cartItem = new Cart
                {
                    UserId = userId.Value, // Gán Id kiểu int vào
                    ProductId = product.Id,
                    Quantity = quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cartItem);
            }

            _context.SaveChanges(); // Lưu thay đổi vào DB
            return RedirectToAction("Index"); 
        }

        // XÓA SẢN PHẨM KHỎI GIỎ
        public IActionResult Remove(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized("Không thể xác thực danh tính người dùng.");
            }

            var cartItem = _context.Carts.FirstOrDefault(p => p.ProductId == id && p.UserId == userId.Value);
            
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges(); // Cập nhật DB sau khi xoá
            }
            
            return RedirectToAction("Index");
        }
    }
}