//using Microsoft.EntityFrameworkCore;
//using WebShopping.Models;

//namespace WebShopping.Repository
//{
//	public class SeedData
//	{
//		public static void SeedingData(DataContext _context)
//		{
//			_context.Database.Migrate();
//			if (_context.Products.Any()) 
//			{
//				CategoryModel macbook = new CategoryModel { Name = "Macbook", Slug = "macbook", Description = "Macbook is Large in the word", Status = 1};
//				CategoryModel pc = new CategoryModel { Name = "PC", Slug = "pc", Description = "PC is Large in the word", Status = 1 };
//				BrandModel apple = new BrandModel { Name = "Apple", Slug = "apple", Description = "Apple is Large in the word", Status = 1 };
//				BrandModel samsung  = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "Samsung is Large in the word", Status = 1 };
//				_context.Products.AddRange(

//					new ProductModel { Name = "Macbook", Slug = "macbook", Description = "Is Best", Image = "1.jpg", Category = macbook, Brand = apple, Price = 20000 },
//					new ProductModel { Name = "Pc", Slug = "pc", Description = "Is Best", Image = "1.jpg", Category = pc, Brand = samsung, Price = 20000 }
//				);
//				_context.SaveChanges();
//			}
//		}
//	}
//}
