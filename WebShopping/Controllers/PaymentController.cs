using Microsoft.AspNetCore.Mvc;
using WebShopping.Models.VnPay;
using WebShopping.Services.Vnpay;

namespace WebShopping.Controllers
{
	public class PaymentController : Controller
	{
		private readonly IVnPayService _vnPayService;
		public PaymentController(IVnPayService vnPayService)
		{

			_vnPayService = vnPayService;
		}

		public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
		{
			var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

			return Redirect(url);
		}
		

	}
}
