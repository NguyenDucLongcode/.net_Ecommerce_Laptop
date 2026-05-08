using ComChienMaDui.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComChienMaDui.ViewComponents
{
    public class ProductsBrandViewComponent : ViewComponent
    {
        private readonly EcommerceLaptopContext _context;

        public ProductsBrandViewComponent(EcommerceLaptopContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selectedBrandId = null)
        {
            var brands = await _context.Brands
                                           .Include(c => c.Products)
                                           .ToListAsync();

            ViewBag.selectedBrandId = selectedBrandId;
            return View(brands);
        }
    }
}