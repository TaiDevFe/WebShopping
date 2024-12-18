﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using WebShopping.Areas.Admin.Repository;
using WebShopping.Models;
using WebShopping.Models.ViewModels;
using WebShopping.Repository;
using WebShopping.Services.Vnpay;

namespace WebShopping.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly IVnPayService _vnPayService;
		private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
		private readonly UserManager<AppUserModel> _userManager;
		public CheckoutController(DataContext context , IEmailSender emailSender , IVnPayService vnPayService, UserManager<AppUserModel> userManager)
		{  
			_dataContext = context;
			_emailSender = emailSender;
			_vnPayService = vnPayService;
			_userManager = userManager;
		}
		public async Task<IActionResult> Index()
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (userEmail == null)
			{
				return RedirectToAction("Login", "Account");
			}
			// Lấy thông tin người dùng từ Identity
			var user = await _userManager.FindByEmailAsync(userEmail);
			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}


			decimal totalPrice = cart.Sum(item => item.Quantity * item.Price );

			//// Tạo model cho view
			var checkoutViewModel = new CheckViewModel
			{
				CartItems = cart,
				UserName = user.UserName,
				Email = user.Email,
				Phone = user.PhoneNumber,
				TotalPrice = totalPrice,
			};

			return View(checkoutViewModel);
			
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
		[HttpGet]
		public IActionResult PaymentCallbackVnpay()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);

			return Json(response);
		}
	}
}
