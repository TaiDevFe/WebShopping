using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopping.Repository;

namespace WebShopping.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		public ProductController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Products.OrderByDescending(p =>p.Id).Include(p =>p.Category).Include(p => p.Brand).ToListAsync());
		}
	}
}

