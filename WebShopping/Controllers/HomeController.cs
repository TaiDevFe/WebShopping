using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Encodings.Web;
using WebShopping.Models;
using WebShopping.Repository;

namespace WebShopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;
		private readonly UserManager<AppUserModel> _userManager;

		public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _dataContext = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var products = _dataContext.Products.Include("Category").Include("Brand").ToList();
            return View(products);
        }
        public async Task<IActionResult> Contact()
        {
            var contact = await _dataContext.Contacts.FirstAsync();
            return View(contact);
        }
        public async Task<IActionResult> Wishlist()
        {
            var wishlist_product = await (from w in _dataContext.Wishlists
                                          join p in _dataContext.Products on w.ProductId equals p.Id
                                          join u in _dataContext.Users on w.UserId equals u.Id
                                          select new { User = u, Product = p, Wishlist = w }).ToListAsync();
            return View(wishlist_product);
        }
        [HttpPost]
        public async Task<IActionResult> AddWishlist(int Id, WishlistModel wishlistmodel)
        {
            var user = await _userManager.GetUserAsync(User);
            var wishlistProduct = new WishlistModel
            {
                ProductId = Id,
                UserId = user.Id,
            };
            _dataContext.Wishlists.Add(wishlistProduct);
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm yêu thích thành công" });
            }
            catch (Exception )
            {
                return StatusCode(500,"An error occurred while adding to wishlist table");
            } 
            
        }
        public async Task<IActionResult> DeleteWishlist(int Id)
        {
            WishlistModel wishlist = await _dataContext.Wishlists.FindAsync(Id);
            _dataContext.Wishlists.Remove(wishlist);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Đã xóa ra khỏi yêu thích";
            return RedirectToAction("Wishlist");
        }
        public async Task<IActionResult> Compare()
		{
			var compare_product = await (from c in _dataContext.Compares
										  join p in _dataContext.Products on c.ProductId equals p.Id
										  join u in _dataContext.Users on c.UserId equals u.Id
										  select new { User = u, Product = p, Compare = c }).ToListAsync();
			return View(compare_product);
		}
		[HttpPost]
		public async Task<IActionResult> AddCompare(int Id)
		{
			var user = await _userManager.GetUserAsync(User);
			var compareProduct = new CompareModel
			{
				ProductId = Id,
				UserId = user.Id,
			};
			_dataContext.Compares.Add(compareProduct);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Thêm so sánh thành công" });
			}
			catch (Exception )
			{
				return StatusCode(500, "An error occurred while adding to compare table");
			}

		}
        public async Task<IActionResult> DeleteCompare(int Id)
        {
            CompareModel compare = await _dataContext.Compares.FindAsync(Id);
            _dataContext.Compares.Remove(compare);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Đã xóa ra khỏi so sánh";
            return RedirectToAction("Compare");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode ==404)
            {
                return View("NotFound");
            }
            else
            {

            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
