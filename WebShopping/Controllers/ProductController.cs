using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopping.Repository;


namespace WebShopping.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var product = await _dataContext.Products.Where(p=>p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToArrayAsync();
            ViewBag.Keyword = searchTerm;
            return View(product);
        }
		public async Task<IActionResult> Details(int Id)
		{
            if (Id == null) return RedirectToAction("Index");
            var productsById = _dataContext.Products.Where(p => p.Id == Id).FirstOrDefault();
            var relatedProducts = await _dataContext.Products.Where(p => p.CategoryId == productsById.CategoryId && p.Id != productsById.Id).Take(4).ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;
			return View(productsById);
		}
	}
}
