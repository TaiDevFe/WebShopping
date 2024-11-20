using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using static System.Net.Mime.MediaTypeNames;

namespace WebShopping.Models
{
	public class CartItemModel
	{
		public long ProductID { get; set; }
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal Total
		{
			get { return Quantity * Price; }
		}
		public string Image {  get; set; }
		public CartItemModel() 
		{
			
		}
		public CartItemModel(ProductModel product)
		{
			ProductID = product.Id;
			ProductName = product.Name;
			Price = product.Price;
			Quantity = 1;
			Image = product.Image;
		}
	}
}
