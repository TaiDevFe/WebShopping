﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebShopping.Repository.Validation;

namespace WebShopping.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên Sản phẩm")]
		public string Name { get; set; }
        public string Slug { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả Sản phẩm")]
		public string Description { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập giá Sản phẩm")]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(8, 1")]
		public decimal Price { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một thương hiệu")]
        public int BrandID { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một danh mục")]
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
        public RatingModel Ratings { get; set; }
        public string Image { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
