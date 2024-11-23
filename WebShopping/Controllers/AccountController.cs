using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebShopping.Models;
using WebShopping.Models.ViewModels;

namespace WebShopping.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
		public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
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
	}
}
