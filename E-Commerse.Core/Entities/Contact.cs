using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerse.Core.Entities
{
    public class Contact : IEntity

    {
        public int Id { get; set; }
        [Display(Name = "Adı")]
        public string Name { get; set; }
        [Display(Name = "Soyadı")]
        public string Surname { get; set; }

        public string? Email { get; set; }
        [Display(Name = "Telefon")]
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Telefon numarası sadece 10 rakamdan oluşmalıdır.")]
        [StringLength(20)]
        public string Phone { get; set; }
        [Display(Name = "Mesaj")]
        public string? Message { get; set; }
        [Display(Name = "Oluşturulma Tarihi"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }
}
