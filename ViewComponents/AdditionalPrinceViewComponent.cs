using ComChienMaDui.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComChienMaDui.ViewComponents
{
    public class AdditionalPrinceViewComponent : ViewComponent
    {
        private readonly EcommerceLaptopContext _context;

        public AdditionalPrinceViewComponent(EcommerceLaptopContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(decimal? minPrice = null, decimal? maxPrice = null)
        {
            var viewModel = new AdditionalProductViewModel
            {
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

     
            // Định nghĩa các khoảng giá
            var priceRanges = new List<PriceRange>
            {
                new PriceRange { Id = 1, Label = "Dưới 10 triệu", Min = 0, Max = 10000000 },
                new PriceRange { Id = 2, Label = "10 - 15 triệu", Min = 10000000, Max = 15000000 },
                new PriceRange { Id = 3, Label = "15 - 20 triệu", Min = 15000000, Max = 20000000 },
                new PriceRange { Id = 4, Label = "20 - 25 triệu", Min = 20000000, Max = 25000000 },
                new PriceRange { Id = 5, Label = "Trên 25 triệu", Min = 25000000, Max = null }
            };

           
            viewModel.PriceRanges = priceRanges;

            return View(viewModel);
        }
    }

    public class AdditionalProductViewModel
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<PriceRange> PriceRanges { get; set; } = new List<PriceRange>();
    }

    public class PriceRange
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
 
    }
}