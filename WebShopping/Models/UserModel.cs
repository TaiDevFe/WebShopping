using System.ComponentModel.DataAnnotations;

namespace WebShopping.Models
{
	public class UserModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Nhập Tên tài khoản")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Nhập Email"),EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password),Required(ErrorMessage = "Nhập mật khẩu")]
		public string Password { get; set; }
	}
}
