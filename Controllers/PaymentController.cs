using ComChienMaDui.Data;
using ComChienMaDui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ComChienMaDui.Controllers
{
    [Authorize] // Nên bật Authorize để tránh lỗi không lấy được UserId
    public class PaymentController : Controller
    {
        private readonly EcommerceLaptopContext _context;

        public PaymentController(EcommerceLaptopContext context)
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
            // Thêm .Include(c => c.Product) để truy QUERY kèm theo thông tin của bảng Product
            return _context.Carts
                           .Include(c => c.Product)
                           .Where(c => c.UserId == userId)
                           .ToList();
        }


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

        [HttpPost]
        public IActionResult PlaceOrder(string FullName, string Phone, string Address, string Note, string PaymentMethod)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized("Phiên đăng nhập hết hạn, vui lòng đăng nhập lại.");
            }

            // 1. Lấy toàn bộ sản phẩm trong giỏ hàng
            var carts = GetCartItems(userId.Value);
            if (carts == null || !carts.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

          

            // A. Bước tạo đơn hàng (Order)
            decimal totalAmount = carts.Sum(c => c.Product.Price * c.Quantity);

            var newOrder = new Order
            {
                UserId = userId.Value,
                TotalAmount = totalAmount,
                Status = "Pending", // Trạng thái đơn hàng lúc vừa đặt mặc định là Pending (Đang chờ xử lý)
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(newOrder); // Add để Entity Framework cấp cho nó 1 ID
            _context.SaveChanges();        // Bắt buộc gọi SaveChanges để lấy OrderId mới tạo

            // B. Khai báo danh sách chi tiết đơn hàng (OrderDetails) và Cập nhật Stock
            var orderDetails = new List<OrderDetail>();

            foreach (var item in carts)
            {
                // Lưu vào chi tiết đơn hàng
                orderDetails.Add(new OrderDetail
                {
                    OrderId = newOrder.Id, // Mã đơn hàng vừa tạo ở bước trên
                    ProductId = item.ProductId,
                    Price = item.Product.Price, // Giá bán tại thời điểm đặt hàng
                    Quantity = item.Quantity
                });

                //// TRỪ STOCK CỦA SẢN PHẨM TRONG KHO
                // Tìm sản phẩm trong db theo ProductId ở trong Table Products để update
                var product = _context.Products.Find(item.ProductId);
                if (product != null && product.Stock != null && product.Stock >= item.Quantity)
                {
                    product.Stock -= item.Quantity; // Thực hiện trừ số lượng
                    _context.Products.Update(product); // Gọi lệnh Update
                }
            }

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
    }
}