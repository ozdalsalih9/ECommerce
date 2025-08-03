using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerse.Core.Entities
{
    public class Comment : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Yorum İçeriği")]
        [Required(ErrorMessage = "Yorum içeriği boş bırakılamaz.")]
        [StringLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir.")]
        public string Content { get; set; }

        [Display(Name = "Puan")]
        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        public int? Rating { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Display(Name = "Onaylandı mı?")]
        public bool IsApproved { get; set; } = false;

        // Kullanıcı ilişkisi
        public int AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        // Ürün ilişkisi
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}