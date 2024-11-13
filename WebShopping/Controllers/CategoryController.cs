using Microsoft.AspNetCore.Mvc;

namespace WebShopping.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
