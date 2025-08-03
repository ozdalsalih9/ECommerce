using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace E_Commerse.Core.Entities
{
    public class Product : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Adı")]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Resim")]
        public string? Image { get; set; }

        [NotMapped]
        [Display(Name = "Ürün Resmi")]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        [Display(Name = "Ek Resimler")]
        public List<IFormFile>? ImageFiles { get; set; }

        [Display(Name = "Fiyat")]
        public decimal? Price { get; set; }

        [Display(Name = "Ürün Kodu")]
        public string? ProductCode { get; set; }

        [Display(Name = "Aktiflik")]
        public bool IsActive { get; set; }

        [Display(Name = "Anasayfa")]
        public bool IsHome { get; set; }

        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }

        [Display(Name = "Kategori")]
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Display(Name = "Marka")]
        public int? BrandId { get; set; }

        [Display(Name = "Marka")]
        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }

        [Display(Name = "Sıra No")]
        public int OrderNo { get; set; }

        [Display(Name = "Oluşturulma Tarihi"), ScaffoldColumn(false)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

        [NotMapped]
        [Display(Name = "Toplam Stok")]
        public int TotalStock => ProductSizes?.Sum(ps => ps.Stock) ?? 0;

        [NotMapped]
        public List<int>? SelectedColorIds { get; set; } = new();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}