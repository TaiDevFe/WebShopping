using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebShopping.Areas.Admin.Repository;
using WebShopping.Models;
using WebShopping.Models.ViewModels;

namespace WebShopping.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
		private readonly IEmailSender _emailSender;
		public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager , IEmailSender emailSender)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
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
					var receiver = "demologin979@gmail.com";
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
		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}
	}
}
