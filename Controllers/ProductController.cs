using ComChienMaDui.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComChienMaDui.Controllers
{
    public class ProductController : Controller
    {
        private readonly EcommerceLaptopContext _context;

        public ProductController(EcommerceLaptopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery, int? categoryId, int? brandId, decimal? minPrice, decimal? maxPrice, string? CurrentSort, int page = 1)
        {
            int pageSize = 9;
            
            // Lấy danh sách sản phẩm và LỌC RA những sản phẩm có dữ liệu lớn hơn 0 (còn hàng)
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Stock != null && p.Stock > 0) // <--- THÊM ĐIỀU KIỆN NÀY VÀO ĐÂY
                .AsQueryable();

            // Xử lý search by Name
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.Name.Contains(searchQuery));
            }

            // Xử lý filter by Category ID 
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Xử lý filter by Brand ID
            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            // Xử lý filter by Price Range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // XỬ LÝ SORT THEO CurrentSort
            switch (CurrentSort)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default:
                    query = query.OrderByDescending(p => p.Id);
                    break;
            }

            // Đếm tổng số lượng và số trang
            int totalItems = await query.CountAsync();
            int totalPages = totalItems > 0 ? (int)Math.Ceiling(totalItems / (double)pageSize) : 1;

            var products = await query
                .Skip((page - 1) * pageSize) // Bỏ qua các sản phẩm của trang trước
                .Take(pageSize)              // Lấy số sản phẩm của trang hiện tại
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CategoryId = categoryId;
            ViewBag.BrandId = brandId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CurrentSort = CurrentSort;

            // Kiểm tra nếu là request AJAX thì chỉ trả về Partial View
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductList", products);
            }

            return View(products);
        }
    }
}