using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShopping.Models;
using WebShopping.Repository;

namespace WebShopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin , Sales")]
    public class ContactController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ContactController(DataContext dataContext , IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var contact = _dataContext.Contacts.ToList();
            return View(contact);
        }
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            ContactModel contact = await _dataContext.Contacts.FirstOrDefaultAsync();
            return View(contact);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var existed_contact = _dataContext.Contacts.FirstOrDefault();
            if (ModelState.IsValid)
            {              
                if (contact.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");
                    string imageName = Guid.NewGuid().ToString() + "_" + contact.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);
                    
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await contact.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_contact.LogoImg = imageName;

                }
                existed_contact.Name = contact.Name;
                existed_contact.Description = contact.Description;
                existed_contact.Phone = contact.Phone;
                existed_contact.Map = contact.Map;
                existed_contact.Email = contact.Email;


                _dataContext.Update(existed_contact);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }

            return View(contact);
        }
    }
}
