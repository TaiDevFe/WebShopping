using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebShopping.Areas.Admin.Repository;
using WebShopping.Models;
using WebShopping.Repository;

namespace WebShopping.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        public CheckoutController(DataContext context , IEmailSender emailSender)
		{  
			_dataContext = context;
			_emailSender = emailSender;
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
				List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
				foreach (var cart in cartItems)
				{
					var orderdetails = new OrderDetails();
					orderdetails.UserName = userEmail;
					orderdetails.OrderCode = ordercode;
					orderdetails.ProductId = cart.ProductID;
					orderdetails.Price = cart.Price;
					orderdetails.Quantity = cart.Quantity;
					_dataContext.Add(orderdetails);
					_dataContext.SaveChanges();
				}
				HttpContext.Session.Remove("Cart");
                //send mail 
                var receiver = "demologin979@gmail.com";
                var subject = "Đặt hàng thành công";
                var mesage = "Đặt hàng thành công , cảm ơn đã sử dụng dịch vụ";
                await _emailSender.SendEmailAsync(receiver, subject, mesage);
                TempData["success"] = "Tạo đơn hàng thành công";
				return RedirectToAction("Index","Cart");
			}
			return View();
		}
	}
}
