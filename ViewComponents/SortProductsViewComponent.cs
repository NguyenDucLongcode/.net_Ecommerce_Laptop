using Microsoft.AspNetCore.Mvc;

namespace ComChienMaDui.ViewComponents
{
    public class SortProductsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string selectedSort = "default")
        {
            ViewBag.SelectedSort = selectedSort;
            return View();
        }
    }
}