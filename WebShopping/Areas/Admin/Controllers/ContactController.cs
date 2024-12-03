using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebShopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin , Sales")]
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
