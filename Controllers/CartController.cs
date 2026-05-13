using ComChienMaDui.Data;
using ComChienMaDui.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ComChienMaDui.Controllers
{
    public class CartController : Controller
    {
        private readonly EcommerceLaptopContext _context;

        public CartController(EcommerceLaptopContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng từ Session ra
        private List<CartItem> GetCartItems()
        {
            var sessionData = HttpContext.Session.GetString("Cart");
            if (sessionData == null) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(sessionData) ?? new List<CartItem>();
        }

        // Lưu giỏ hàng vào Session
        private void SaveCartSession(List<CartItem> ls)
        {
            var sessionData = JsonSerializer.Serialize(ls);
            HttpContext.Session.SetString("Cart", sessionData);
        }

        // GIAO DIỆN CHÍNH CỦA GIỎ HÀNG
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // XỬ LÝ THÊM VÀO GIỎ HÀNG
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var product = _context.Products.Find(id); // Lưu ý: Nếu Database mày tên cột khác (vd: MaSP) thì sửa lại chữ Id nha
            if (product == null) return NotFound("Không tìm thấy sản phẩm");

            var cart = GetCartItems();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                item.Quantity += quantity; // Nếu có rồi thì cộng dồn số lượng
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id, // Đổi 'Id' thành tên cột ID sản phẩm của mày nếu cần
                    ProductName = product.Name ?? "Sản phẩm ẩn danh", // Đổi 'Name' thành tên cột Tên sản phẩm
                    Price = product.Price, // Đổi 'Price' thành tên cột Giá
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl // Đổi 'ImageUrl' thành tên cột Hình ảnh
                });
            }

            SaveCartSession(cart);
            return RedirectToAction("Index"); // Thêm xong nhảy về trang Giỏ hàng
        }

        // XÓA SẢN PHẨM KHỎI GIỎ
        public IActionResult Remove(int id)
        {
            var cart = GetCartItems();
            var item = cart.SingleOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCartSession(cart);
            }
            return RedirectToAction("Index");
        }
    }
}