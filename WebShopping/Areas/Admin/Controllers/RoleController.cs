using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopping.Models;
using WebShopping.Repository;

namespace WebShopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin , Sales")]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(DataContext context , RoleManager<IdentityRole> roleManager)
        {

            _dataContext = context;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Roles.OrderByDescending(p=>p.Id).ToArrayAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            } 
             var role = await _roleManager.FindByIdAsync(id);   
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id ,IdentityRole model)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                } 
                role.Name = model.Name;  
                try
                {
                    await _roleManager.UpdateAsync(role);
                    TempData["success"] = "Cập nhật role thành công !";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi cập nhật Role");
                } 
                
            }
            return View(model ?? new IdentityRole { Id = id });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return Redirect("Index");
        }
        [HttpGet]     
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound();
            } 
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            } 
                try
            {
                await _roleManager.DeleteAsync(role);
                TempData["success"] = "Role đã được xóa thành công";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi xóa role");
            }
            return RedirectToAction("Index","Role");
        }
        
    }
}
