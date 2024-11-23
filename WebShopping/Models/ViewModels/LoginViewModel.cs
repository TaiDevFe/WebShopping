using System.ComponentModel.DataAnnotations;

namespace WebShopping.Models.ViewModels
{
	public class LoginViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Nhập Tên tài khoản")]
		public string Username { get; set; }
		[DataType(DataType.Password), Required(ErrorMessage = "Nhập mật khẩu")]
		public string Password { get; set; }

		public string ReturnUrl { get; set; }
	}
}
