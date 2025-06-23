using System.ComponentModel.DataAnnotations;

namespace E_Commerse.Core.Entities
{
    public class Color : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Renk Adı")]
        public string Name { get; set; }

        [Display(Name = "Renk Kodu")]
        public string Code { get; set; } 

        [Display(Name = "Aktiflik")]
        public bool IsActive { get; set; } = true;
    }
}