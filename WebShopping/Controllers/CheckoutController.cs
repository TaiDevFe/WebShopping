using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebShopping.Models;
using WebShopping.Repository;

namespace WebShopping.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly DataContext _dataContext;
		public CheckoutController(DataContext context)
		{  
			_dataContext = context;
		}
		public async Task<IActionResult> Checkout()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (userEmail == null)
			{
				return RedirectToAction("Login", "Account");
			}
			else
			{
				var ordercode = Guid.NewGuid().ToString();
				var orderItem = new OrderModel();
				orderItem.OrderCode = ordercode;
				orderItem.UserName = userEmail;
				orderItem.Status = 1;
				orderItem.CreatedDate = DateTime.Now;
				_dataContext.Add(orderItem);
				_dataContext.SaveChanges();
				TempData["success"] = "Đơn hàng đã được tạo";
				return RedirectToAction("Index","Cart");
			}
			return View();
		}
	}
}
