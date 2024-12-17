using System.ComponentModel.DataAnnotations;

namespace WebShopping.Models
{
	public class CouponModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên khuyến mãi")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập mô tả")]
		public string Description { get; set; }
		public DateTime DateStart { get; set; }
		public DateTime DateExpried { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập số lượng")]
		public int Quantity { get; set; }
		public int Status { get; set; }
	}
}
