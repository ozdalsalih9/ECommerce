using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerse.Core.Entities
{
    public class AppUser : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "İsim")]
        public string Name { get; set; }
        [Display(Name = "Soyisim")]
        public string Surname { get; set; }
        [Display(Name = "Mail Adresi")]
        public string Email { get; set; }
        [Display(Name = "Şifre")]

        public string Password { get; set; }
        [Display(Name = "Telefon")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Telefon numarası sadece 10 rakamdan oluşmalıdır.")]
        [StringLength(20)]
        public string? Phone { get; set; }
        [Display(Name = "Aktiflik")]
        public bool IsActive { get; set; }
        [Display(Name = "Adminlik")]
        public bool IsAdmin { get; set; }
        [Display(Name = "Oluşturulma Tarihi"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [ScaffoldColumn(false)]
        public Guid? UserGuid { get; set; } = Guid.NewGuid();

        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
