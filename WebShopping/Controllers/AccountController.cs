using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebShopping.Areas.Admin.Repository;
using WebShopping.Models;
using WebShopping.Models.ViewModels;
using WebShopping.Repository;

namespace WebShopping.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly DataContext _dataContext;
		
		public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager , IEmailSender emailSender, DataContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_dataContext = context;
		}
		
		public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl});
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{
			if(ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false,false);
				if(result.Succeeded)
				{
					TempData["success"] = "Đăng nhập thành công";
					var receiver = "thontanhanh@gmail.com";
					var subject = "Đăng nhập trên thiết bị thành công";
					var mesage = "Đăng nhập thành công";
					await _emailSender.SendEmailAsync(receiver, subject, mesage);
                    return Redirect(loginViewModel.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng");
			}	
			return View(loginViewModel);
		}	
		public IActionResult Create()
		{
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> History()
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			var orders = await _dataContext.Orders.Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();
			ViewBag.UserEmail = userEmail;
			return View(orders);
		}
		[HttpPost]
		public async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser,user.Password);
				if (result.Succeeded)
				{
					TempData["success"] = "Tạo user thành công";
					return Redirect("/account/login");
				} 
				foreach(IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				} 										
			} 				
			return View(user);
		}
		public async Task<IActionResult> CancelOrder(string ordercode)
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}
			try
			{
				var order = await _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
				order.Status = 3;
				_dataContext.Update(order);
				await _dataContext.SaveChangesAsync();


			}
			catch (Exception ex)
			{
				return BadRequest("An error occured while cancelling the order");

			}
			return RedirectToAction("History", "Account");
		}
		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateAccount(AppUserModel appUser)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userById = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (userById == null)
			{
				return NotFound();
			}
			else
			{
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(userById, appUser.PasswordHash);
				userById.PasswordHash = passwordHash;
				_dataContext.Update(userById);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Update Account Success";

			}

			return RedirectToAction("UpdateAccount", "Account");
			return View();
		}
	}
}
