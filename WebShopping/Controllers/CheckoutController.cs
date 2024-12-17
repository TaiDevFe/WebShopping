using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
				var shippingPriceCookie = Request.Cookies["ShippingPrice"];
				decimal shippingPrice = 0;
				var coupon_code = Request.Cookies["CouponTitle"];
				if (!string.IsNullOrEmpty(shippingPriceCookie))
				{
					var shippingPriceJson = shippingPriceCookie;
					shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
				}
				orderItem.ShippingCost = shippingPrice;
				orderItem.CouponCode = coupon_code;
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
					var product = await _dataContext.Products.Where(p=>p.Id == cart.ProductID).FirstOrDefaultAsync();
					product.Quantity -= cart.Quantity;
					product.Sold += cart.Quantity;
					_dataContext.Add(orderdetails);
					_dataContext.SaveChanges();
				}
				HttpContext.Session.Remove("Cart");
				//send mail 
				var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var mesage = "Đặt hàng thành công , cảm ơn đã sử dụng dịch vụ";
                await _emailSender.SendEmailAsync(receiver, subject, mesage);
                TempData["success"] = "Tạo đơn hàng thành công";
				return RedirectToAction("History","Account");
			}
			return View();
		}
	}
}
