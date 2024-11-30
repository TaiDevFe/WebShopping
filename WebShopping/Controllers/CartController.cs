﻿using Microsoft.AspNetCore.Mvc;
using WebShopping.Models;
using WebShopping.Models.ViewModels;
using WebShopping.Repository;

namespace WebShopping.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }
		public IActionResult Checkout()
		{
			return View("~/Views/Checkout/Index.cshtml");
		}
        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.Where(c => c.ProductID == Id).FirstOrDefault();
            if (cartItems == null) { 
                cart.Add(new CartItemModel (product));
            }
            else
            {
                cartItems.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);
            //TempData["success"] = "Thêm vào giỏ hàng thành công";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart"); 
            CartItemModel cartItem = cart.Where(c => c.ProductID == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p=>p.ProductID == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
			}
            else
            {
				HttpContext.Session.SetJson("Cart", cart);
			}
            TempData["success"] = "Decrease Item quanity of cart Successfuly";
            return RedirectToAction("Index");
        }
		public async Task<IActionResult> Increase(int Id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			CartItemModel cartItem = cart.Where(c => c.ProductID == Id).FirstOrDefault();
			if (cartItem.Quantity >= 1)
			{
				++cartItem.Quantity;
			}
			else
			{
				cart.RemoveAll(p => p.ProductID == Id);
			}
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}
            TempData["success"] = "Increase Item quanity of cart Successfuly";
            return RedirectToAction("Index");
		}
        public async Task<IActionResult> Remove(int Id)
        {
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p=>p.ProductID == Id);
            if (cart.Count == 0)
            {
				HttpContext.Session.Remove("Cart");
			} 
            else
            {
				HttpContext.Session.SetJson("Cart", cart);
			}
            TempData["success"] = "Remove Item of cart Successfuly";
            return RedirectToAction("Index");       
		}
        public async Task<IActionResult> Clear()
        {
			HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear All Cart Successfuly";
            return RedirectToAction("Index");
		}
	}	
}
