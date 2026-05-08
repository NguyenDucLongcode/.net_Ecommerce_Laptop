using ComChienMaDui.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComChienMaDui.ViewComponents
{
    public class ProductsCategoriesViewComponent : ViewComponent
    {
        private readonly EcommerceLaptopContext _context;

        public ProductsCategoriesViewComponent(EcommerceLaptopContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selectedCategoryId = null)
        {
            var categories = await _context.Categories
                                           .Include(c => c.Products)
                                           .ToListAsync();

            ViewBag.SelectedCategoryId = selectedCategoryId;
            return View(categories);
        }
    }
}