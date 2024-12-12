using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopping.Models;
using WebShopping.Repository;

namespace WebShopping.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin , Sales")]
	public class OrderController : Controller
	{
		private readonly DataContext _dataContext;
		public OrderController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
		}
		public async Task<IActionResult> ViewOrder(string ordercode)
		{
			var DetailsOder = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == ordercode).ToListAsync();
			var ShippingCost = _dataContext.Orders.Where(o=>o.OrderCode == ordercode).First();
			ViewBag.ShippingCost = ShippingCost.ShippingCost;
			return View(DetailsOder);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateOrder(string ordercode, int status)
		{
			var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
			if (order == null)
			{
				return NotFound();
			}
			order.Status = status;
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Lỗi cập nhật trạng thái đơn hàng");
			}
		}
		[HttpGet]
        public async Task<IActionResult> Delete(string ordercode)
        {

            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode.ToString() == ordercode);
            if (order == null)
            {
                TempData["error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            try
            {
                _dataContext.Orders.Remove(order);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Đã xóa đơn hàng thành công.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Xóa đơn hàng thất bại: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

    }
}
